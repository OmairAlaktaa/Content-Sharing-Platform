using ContentShare.Domain.Entities;
using ContentShare.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ContentShare.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>(options)
{
    public DbSet<MediaContent> MediaContents => Set<MediaContent>();
    public DbSet<Rating> Ratings => Set<Rating>();
    public DbSet<Report> Reports => Set<Report>();


    protected override void OnModelCreating(ModelBuilder b)
    {
        base.OnModelCreating(b);

        b.Entity<MediaContent>(e =>
        {
            e.Property(p => p.Title).IsRequired().HasMaxLength(200);
            e.Property(p => p.ThumbnailUrl).IsRequired();
            e.Property(p => p.ContentUrl).IsRequired();
            e.HasMany(x => x.Ratings)
             .WithOne()
             .HasForeignKey(r => r.MediaContentId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        b.Entity<Rating>(e =>
        {
            e.Property(p => p.Score).IsRequired();
            e.Property(p => p.Comment).HasMaxLength(1000);

            e.HasOne<AppUser>()
             .WithMany()
             .HasForeignKey(r => r.UserId)
             .OnDelete(DeleteBehavior.Cascade);

            e.HasIndex(p => new { p.MediaContentId, p.UserId }).IsUnique();

            e.ToTable(t =>
            {
                t.HasCheckConstraint("CK_Ratings_Score_Range", "\"Score\" >= 1 AND \"Score\" <= 5");
            });

        });

        b.Entity<Report>(e =>
        {
            e.Property(p => p.Reason).IsRequired();
            e.Property(p => p.Note).HasMaxLength(1000);
            e.HasIndex(p => new { p.RatingId, p.ReporterUserId }).IsUnique();
        });
    }
}
