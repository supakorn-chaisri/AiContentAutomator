using AiContent.Application.Common.Interfaces;
using AiContent.Domain.Entities;
using FluentValidation;
using MediatR;

namespace AiContent.Application.Features.Feeds.AddFeed;

// 1. DTO ฝั่งรับข้อมูล (Request)
public record AddFeedCommand(string Name, string Url, string Category) : IRequest<int>;

// 2. 🔥 กฎการ Validation (FluentValidation จะถูก ValidationBehavior รันให้อัตโนมัติ)
public class AddFeedCommandValidator : AbstractValidator<AddFeedCommand>
{
    public AddFeedCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Url).NotEmpty().Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _))
            .WithMessage("รูปแบบ URL ไม่ถูกต้อง");
        RuleFor(x => x.Category).NotEmpty().Must(c => c is "Finance" or "Tech")
            .WithMessage("หมวดหมู่ต้องเป็น Finance หรือ Tech เท่านั้น");
    }
}

// 3. Business Logic Handler (เข้าถึงตรงนี้แปลว่าข้อมูลผ่านการตรวจและทำ Log เรียบร้อย)
public class AddFeedCommandHandler(IApplicationDbContext context) : IRequestHandler<AddFeedCommand, int>
{
    public async Task<int> Handle(AddFeedCommand request, CancellationToken cancellationToken)
    {
        var feed = new RssFeed
        {
            Name = request.Name,
            Url = request.Url,
            Category = request.Category
        };

        context.RssFeeds.Add(feed);
        await context.SaveChangesAsync(cancellationToken);

        return feed.Id; // ได้ Id ที่ Postgres เจนให้อัตโนมัติกลับไป
    }
}