namespace AiContent.Domain.Entities;

public class NewsArticle
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public int FeedId { get; set; }
    public required string Title { get; set; }
    public required string SourceUrl { get; set; }
    public DateTimeOffset? PublishedAt { get; set; }
    public string? RawContent { get; set; }
    public string? AiSummary { get; set; }
    public string Status { get; set; } = "Pending"; // Pending, Approved, Posted
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    // Navigation Property
    public RssFeed? Feed { get; set; }
}