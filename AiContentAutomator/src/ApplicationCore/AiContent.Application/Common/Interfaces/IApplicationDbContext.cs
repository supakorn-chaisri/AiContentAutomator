using AiContent.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AiContent.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<RssFeed> RssFeeds { get; }
    DbSet<NewsArticle> NewsArticles { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}