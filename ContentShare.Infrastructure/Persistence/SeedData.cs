using ContentShare.Domain.Entities;
using ContentShare.Domain.Enums;
using ContentShare.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ContentShare.Infrastructure.Persistence;

public static class SeedData
{
    public static async Task SeedAsync(IServiceProvider services, CancellationToken ct = default)
    {
        var db = services.GetRequiredService<AppDbContext>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
        var userManager = services.GetRequiredService<UserManager<AppUser>>();

        if (db.Database.IsRelational())
        {
            await db.Database.MigrateAsync(ct);
        }

        var roles = new[] { "Admin", "User" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole<Guid>(role));
            }
        }


        var joe = await EnsureUserAsync(userManager, "joe", "joe@test.com", "P@ssw0rd1!", new[] { "Admin" });
        var alice = await EnsureUserAsync(userManager, "alice", "alice@test.com", "P@ssw0rd1!", new[] { "User" });
        var bob = await EnsureUserAsync(userManager, "bob", "bob@test.com", "P@ssw0rd1!", new[] { "User" });

        joe = await userManager.FindByEmailAsync("joe@test.com") ?? throw new InvalidOperationException("Seed user joe missing.");
        alice = await userManager.FindByEmailAsync("alice@test.com") ?? throw new InvalidOperationException("Seed user alice missing.");
        bob = await userManager.FindByEmailAsync("bob@test.com") ?? throw new InvalidOperationException("Seed user bob missing.");

        if (!await db.MediaContents.AnyAsync(ct))
        {
            var now = DateTime.UtcNow;

            var contents = new[]
            {
                new MediaContent
                {
                    Title = "Indie Game Alpha",
                    Description = "Prototype build and teaser",
                    Category = MediaCategory.Game,
                    ThumbnailUrl = "https://picsum.photos/seed/game1/200",
                    ContentUrl = "https://example.com/game1",
                    CreatedAt = now.AddDays(-10)
                },
                new MediaContent
                {
                    Title = "Trailer Video",
                    Description = "Launch trailer",
                    Category = MediaCategory.Video,
                    ThumbnailUrl = "https://picsum.photos/seed/video1/200",
                    ContentUrl = "https://example.com/video1",
                    CreatedAt = now.AddDays(-9)
                },
                new MediaContent
                {
                    Title = "Cover Artwork",
                    Description = "Key art pack",
                    Category = MediaCategory.Artwork,
                    ThumbnailUrl = "https://picsum.photos/seed/art1/200",
                    ContentUrl = "https://example.com/art1",
                    CreatedAt = now.AddDays(-8)
                },
                new MediaContent
                {
                    Title = "TTT Track 01",
                    Description = "Theme music",
                    Category = MediaCategory.Music,
                    ThumbnailUrl = "https://picsum.photos/seed/music1/200",
                    ContentUrl = "https://example.com/music1",
                    CreatedAt = now.AddDays(-7)
                },
                new MediaContent
                {
                    Title = "Level Preview",
                    Description = "Early level walkthrough",
                    Category = MediaCategory.Video,
                    ThumbnailUrl = "https://picsum.photos/seed/video2/200",
                    ContentUrl = "https://example.com/video2",
                    CreatedAt = now.AddDays(-6)
                }
            };

            await db.MediaContents.AddRangeAsync(contents, ct);
            await db.SaveChangesAsync(ct);
        }

        if (!await db.Ratings.AnyAsync(ct))
        {
            var now = DateTime.UtcNow;

            var c1 = await db.MediaContents.OrderBy(c => c.CreatedAt).Skip(0).FirstAsync(ct);
            var c2 = await db.MediaContents.OrderBy(c => c.CreatedAt).Skip(1).FirstAsync(ct);
            var c3 = await db.MediaContents.OrderBy(c => c.CreatedAt).Skip(2).FirstAsync(ct);
            var c4 = await db.MediaContents.OrderBy(c => c.CreatedAt).Skip(3).FirstAsync(ct);
            var c5 = await db.MediaContents.OrderBy(c => c.CreatedAt).Skip(4).FirstAsync(ct);

            var ratings = new List<Rating>
            {
                new Rating { MediaContentId = c1.Id, UserId = joe.Id,   Score = 5, Comment = "Loved it",           CreatedAt = now.AddDays(-5) },
                new Rating { MediaContentId = c1.Id, UserId = joe.Id,   Score = 3, Comment = "On second thought…", CreatedAt = now.AddDays(-4) },

                new Rating { MediaContentId = c2.Id, UserId = joe.Id,   Score = 4, Comment = "Solid",              CreatedAt = now.AddDays(-3) },
                new Rating { MediaContentId = c2.Id, UserId = alice.Id, Score = 5, Comment = "Fantastic",          CreatedAt = now.AddDays(-2) },

                new Rating { MediaContentId = c3.Id, UserId = alice.Id, Score = 2, Comment = "Not my style",       CreatedAt = now.AddDays(-2) },
                new Rating { MediaContentId = c4.Id, UserId = alice.Id, Score = 4, Comment = "Chill vibes",        CreatedAt = now.AddDays(-1) },

                new Rating { MediaContentId = c5.Id, UserId = bob.Id,   Score = 5, Comment = "Great reference!",   CreatedAt = now.AddDays(-1) }
            };

            await db.Ratings.AddRangeAsync(ratings, ct);
            await db.SaveChangesAsync(ct);
        }

        async Task UpdateRatingCountAsync(AppUser u)
        {
            u.RatingCount = await db.Ratings.CountAsync(r => r.UserId == u.Id, ct);
            await userManager.UpdateAsync(u);
        }

        await UpdateRatingCountAsync(joe);
        await UpdateRatingCountAsync(alice);
        await UpdateRatingCountAsync(bob);
    }

    private static async Task<AppUser> EnsureUserAsync(
        UserManager<AppUser> userManager,
        string username,
        string email,
        string password,
        IEnumerable<string> roles)
    {
        var existing = await userManager.FindByEmailAsync(email);
        if (existing != null) return existing;

        var user = new AppUser
        {
            UserName = username,
            Email = email,
            EmailConfirmed = true,
            CreatedAt = DateTime.UtcNow
        };

        var create = await userManager.CreateAsync(user, password);
        if (!create.Succeeded)
        {
            var msg = string.Join("; ", create.Errors.Select(e => e.Description));
            throw new Exception("Failed to create seed user: " + msg);
        }

        if (roles.Any())
        {
            var addRoles = await userManager.AddToRolesAsync(user, roles);
            if (!addRoles.Succeeded)
            {
                var msg = string.Join("; ", addRoles.Errors.Select(e => e.Description));
                throw new Exception("Failed to add roles to seed user: " + msg);
            }
        }

        return user;
    }
}
