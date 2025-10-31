using ContentShare.Domain.Entities;
using ContentShare.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ContentShare.Infrastructure.Persistence;

public class AppDbContext : IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>
{
    public DbSet<MediaContent> MediaContents => Set<MediaContent>();
    public DbSet<Rating> Ratings => Set<Rating>();

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder b)
    {
        base.OnModelCreating(b);

        b.Entity<MediaContent>(e =>
        {
            e.Property(p => p.Title).IsRequired().HasMaxLength(200);
            e.Property(p => p.ThumbnailUrl).IsRequired();
            e.Property(p => p.ContentUrl).IsRequired();
        });

        b.Entity<Rating>(e =>
        {
            e.Property(p => p.Score).IsRequired();
            e.HasIndex(p => new { p.MediaContentId, p.UserId, p.CreatedAt });
        });
    }
}
