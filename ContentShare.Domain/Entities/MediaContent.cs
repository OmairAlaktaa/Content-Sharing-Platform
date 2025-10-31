using ContentShare.Domain.Common;
using ContentShare.Domain.Enums;

namespace ContentShare.Domain.Entities;

public class MediaContent : BaseEntity
{
    public string Title { get; set; } = default!;
    public string? Description { get; set; }
    public MediaCategory Category { get; set; } // game|video|artwork|music
    public string ThumbnailUrl { get; set; } = default!;
    public string ContentUrl { get; set; } = default!;

    // Relations
    public ICollection<Rating> Ratings { get; set; } = new List<Rating>();
}
