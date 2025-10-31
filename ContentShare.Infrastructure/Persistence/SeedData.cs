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

        await db.Database.MigrateAsync(ct);

        #region Sample Roles
        var roles = new[] { "Admin", "User" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole<Guid>(role));
        }
        #endregion

        #region Sample Users
        await EnsureUserAsync(userManager, "joe", "joe@test.com", "P@ssw0rd1!", new[] { "Admin" });
        await EnsureUserAsync(userManager, "alice", "alice@test.com", "P@ssw0rd1!", new[] { "User" });
        await EnsureUserAsync(userManager, "bob", "bob@test.com", "P@ssw0rd1!", new[] { "User" });
        #endregion

        #region Sample media content
        if (!await db.MediaContents.AnyAsync(ct))
        {
            var now = DateTime.UtcNow;
            var items = new[]
            {
                new MediaContent { Title = "Indie Game Alpha", Category = MediaCategory.Game, ThumbnailUrl = "https://picsum.photos/seed/game1/200", ContentUrl = "https://example.com/game1", Description = "Prototype", CreatedAt = now },
                new MediaContent { Title = "Trailer Video", Category = MediaCategory.Video, ThumbnailUrl = "https://picsum.photos/seed/video1/200", ContentUrl = "https://example.com/video1", Description = "Launch trailer", CreatedAt = now },
                new MediaContent { Title = "Cover Artwork", Category = MediaCategory.Artwork, ThumbnailUrl = "https://picsum.photos/seed/art1/200", ContentUrl = "https://example.com/art1", Description = "Key art", CreatedAt = now },
                new MediaContent { Title = "OST Track 01", Category = MediaCategory.Music, ThumbnailUrl = "https://picsum.photos/seed/music1/200", ContentUrl = "https://example.com/music1", Description = "Theme", CreatedAt = now },
                new MediaContent { Title = "Level Preview", Category = MediaCategory.Video, ThumbnailUrl = "https://picsum.photos/seed/video2/200", ContentUrl = "https://example.com/video2", Description = "Level 2", CreatedAt = now }
            };
            db.MediaContents.AddRange(items);
            await db.SaveChangesAsync(ct);
        }
        #endregion
    }

    private static async Task EnsureUserAsync(
        UserManager<AppUser> userManager,
        string username,
        string email,
        string password,
        IEnumerable<string> roles)
    {
        var existing = await userManager.FindByEmailAsync(email);
        if (existing != null) return;

        var user = new AppUser
        {
            UserName = username,
            Email = email,
            EmailConfirmed = true,
            CreatedAt = DateTime.UtcNow
        };

        var create = await userManager.CreateAsync(user, password);
        if (!create.Succeeded)
            throw new Exception("Failed to create seed user: " + string.Join("; ", create.Errors.Select(e => e.Description)));

        if (roles.Any())
        {
            var addRoles = await userManager.AddToRolesAsync(user, roles);
            if (!addRoles.Succeeded)
                throw new Exception("Failed to add roles to seed user: " + string.Join("; ", addRoles.Errors.Select(e => e.Description)));
        }
    }
}
