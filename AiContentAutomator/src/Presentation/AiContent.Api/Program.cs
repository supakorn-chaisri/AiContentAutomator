using AiContent.Application;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

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


    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
    }

    // เปิดใช้งาน Serilog Request Logging เพื่อเก็บ Log ของ HTTP Requests อัตโนมัติ
    app.UseSerilogRequestLogging();

    app.UseHttpsRedirection();


    // เทสยิงเล่นสั้นๆ ดูว่าระบบทำงานไหม
    app.MapGet("/health", () => Results.Ok(new { Status = "Healthy", Version = "10.0" }));

    app.Run();

}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
