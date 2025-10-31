using ContentShare.Domain.Entities;
using ContentShare.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace ContentShare.Infrastructure.Persistence;

public static class SeedData
{
    public static async Task SeedAsync(AppDbContext db, CancellationToken ct = default)
    {
        await db.Database.MigrateAsync(ct);

        if (!await db.MediaContents.AnyAsync(ct))
        {
            var now = DateTime.UtcNow;
            var items = new[]
            {
                new MediaContent { Title = "Indie Game Alpha", Category = MediaCategory.Game, ThumbnailUrl = "https://picsum.photos/seed/game1/200", ContentUrl = "https://example.com/game1", Description="Prototype", CreatedAt = now },
                new MediaContent { Title = "Trailer Video", Category = MediaCategory.Video, ThumbnailUrl = "https://picsum.photos/seed/video1/200", ContentUrl = "https://example.com/video1", Description="Launch trailer", CreatedAt = now },
                new MediaContent { Title = "Cover Artwork", Category = MediaCategory.Artwork, ThumbnailUrl = "https://picsum.photos/seed/art1/200", ContentUrl = "https://example.com/art1", Description="Key art", CreatedAt = now },
                new MediaContent { Title = "OST Track 01", Category = MediaCategory.Music, ThumbnailUrl = "https://picsum.photos/seed/music1/200", ContentUrl = "https://example.com/music1", Description="Theme", CreatedAt = now },
                new MediaContent { Title = "Level Preview", Category = MediaCategory.Video, ThumbnailUrl = "https://picsum.photos/seed/video2/200", ContentUrl = "https://example.com/video2", Description="Level 2", CreatedAt = now }
            };
            db.MediaContents.AddRange(items);
            await db.SaveChangesAsync(ct);
        }
    }
}
