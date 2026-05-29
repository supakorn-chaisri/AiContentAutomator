using AiContent.Application.Behaviors;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace AiContent.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = typeof(DependencyInjection).Assembly;

        // 1. ลงทะเบียน MediatR ติดตั้ง Behaviors แบบ Open Generics 
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(assembly);

            // ลำดับการใส่มีความสำคัญ: เริ่มบนลงล่าง (Logging คลุม Validation อีกที)
            config.AddOpenBehavior(typeof(LoggingBehavior<,>));
            config.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        // 2. ลงทะเบียน FluentValidation สแกนหาพวก Validator ทั้งหมดใน Assembly นี้
        services.AddValidatorsFromAssembly(assembly);

        return services;
    }
}