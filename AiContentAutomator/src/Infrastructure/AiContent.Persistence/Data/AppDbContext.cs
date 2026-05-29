using AiContent.Application.Common.Interfaces;
using AiContent.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AiContent.Persistence.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options), IApplicationDbContext
{
    public DbSet<RssFeed> RssFeeds => Set<RssFeed>();
    public DbSet<NewsArticle> NewsArticles => Set<NewsArticle>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // กำหนดคอนฟิกให้ตาราง RssFeed
        modelBuilder.Entity<RssFeed>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Url).IsUnique(); // URL แหล่งข่าวห้ามซ้ำ
            entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Url).HasMaxLength(500).IsRequired();
            entity.Property(e => e.Category).HasMaxLength(50).IsRequired();
        });

        // กำหนดคอนฟิกให้ตาราง NewsArticle
        modelBuilder.Entity<NewsArticle>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.SourceUrl).IsUnique(); // ลิงก์ข่าวห้ามซ้ำ (ป้องกันเก็บข่าวซ้ำซ้อน)
            entity.Property(e => e.Title).HasMaxLength(500).IsRequired();
            entity.Property(e => e.SourceUrl).HasMaxLength(500).IsRequired();
            entity.Property(e => e.Status).HasMaxLength(20).IsRequired();

            // Setup One-to-Many Relationship
            entity.HasOne(d => d.Feed)
                .WithMany(p => p.NewsArticles)
                .HasForeignKey(d => d.FeedId)
                .OnDelete(DeleteBehavior.Cascade); // ถ้าลบ Feed ให้ลบข่าวภายใต้ Feed นั้นทั้งหมด
        });
    }
}