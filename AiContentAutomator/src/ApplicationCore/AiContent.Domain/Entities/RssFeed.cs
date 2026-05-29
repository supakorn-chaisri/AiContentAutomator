namespace AiContent.Domain.Entities;

public class RssFeed
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Url { get; set; }
    public required string Category { get; set; } // 'Finance' หรือ 'Tech'
    public bool IsActive { get; set; } = true;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    // Relationship (Navigation Property)
    public ICollection<NewsArticle> NewsArticles { get; set; } = new List<NewsArticle>();
}