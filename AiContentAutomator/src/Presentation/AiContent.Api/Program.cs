using AiContent.Api.Endpoints;
using AiContent.Application;
using AiContent.Persistence;
using Scalar.AspNetCore;
using Serilog;

// โหลดไฟล์ .env 
DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();


// ตั้งค่า Serilog สำหรับระบบ Log 
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console() // Console เท่านั้น
    .CreateLogger();

builder.Host.UseSerilog(); // ใช้ Serilog แทนตัวสแตนดาร์ด
try
{
    Log.Information("Starting web application host...");

    // เรียกใช้ Extension Method จากชั้น Application ที่เซ็ตอัป MediatR + FluentValidation ไว้
    builder.Services.AddApplication();

    builder.Services.AddPersistence(builder.Configuration);


    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();

        // 2. UI ของ Scalar ที่/scalar/v1
        app.MapScalarApiReference(options =>
        {
            options.WithTitle("AiContent Automator API")
                   .WithTheme(ScalarTheme.DeepSpace) // เลือกธีมได้ตามชอบ (เช่น DeepSpace, Mars, Nebula)
                   .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
        });
    }

    // เปิดใช้งาน Serilog Request Logging เพื่อเก็บ Log ของ HTTP Requests อัตโนมัติ
    app.UseSerilogRequestLogging();

    app.UseHttpsRedirection();

    // API Endpoints
    app.MapFeedEndpoints();


    // เทสยิงเล่นสั้นๆ ดูว่าระบบทำงานไหม
    app.MapGet("/health", () => Results.Ok(new { Status = "Healthy", Version = "10.0" }));

    app.Run();

}
catch (Exception ex) when (ex is not HostAbortedException)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
