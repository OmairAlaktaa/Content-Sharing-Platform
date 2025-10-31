using Microsoft.AspNetCore.Identity;

namespace ContentShare.Infrastructure.Identity;

public class AppUser : IdentityUser<Guid>
{
    public int RatingCount { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastLogin { get; set; }
}
