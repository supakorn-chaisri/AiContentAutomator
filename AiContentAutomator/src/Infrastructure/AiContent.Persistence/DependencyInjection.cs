using AiContent.Application.Common.Interfaces;
using AiContent.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AiContent.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Database")
            ?? throw new ArgumentNullException(nameof(configuration), "Database connection string is missing.");

        // ลงทะเบียน DbContext ร่วมกับ Npgsql
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString, npgsqlOptions =>
            {
                // เปิดฟีเจอร์คำสั่งเชื่อมต่อใหม่อัตโนมัติหาก Network หลุดชั่วคราว (Resilience ท่ามาตรฐานบริษัทใหญ่)
                npgsqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorCodesToAdd: null);
            }));


        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<AppDbContext>());


        return services;
    }
}