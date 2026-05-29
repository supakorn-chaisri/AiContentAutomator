using AiContent.Application.Features.Feeds.AddFeed;
using MediatR;

namespace AiContent.Api.Endpoints;

public static class FeedEndpoints
{
    public static void MapFeedEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/feeds").WithTags("Feeds");

        group.MapPost("/", async (AddFeedCommand command, ISender sender) =>
        {
            var feedId = await sender.Send(command);
            return Results.Created($"/api/feeds/{feedId}", new { Id = feedId });
        })
        .WithName("AddFeed") // กำหนด Operation ID ให้ระบบ
        .WithSummary("เพิ่มแหล่งข้อมูล RSS Feed ใหม่") // คำอธิบายสั้นๆ หัวข้อ
        .WithDescription("ใช้สำหรับการลงทะเบียน URL ของ RSS Feed เข้าสู่ระบบ เพื่อให้ Background Worker ดึงข้อมูลข่าวไปประมวลผลต่อ") // คำอธิบายละเอียด
        .Produces(StatusCodes.Status201Created) // บอกหน้าบ้านว่าถ้าสำเร็จจะส่ง 201 กลับไปนะ
        .Produces(StatusCodes.Status400BadRequest); // บอกหน้าบ้านว่าถ้าเจอ Validation Error จะส่ง 400 กลับไป

    }
}