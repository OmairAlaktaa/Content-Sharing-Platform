using ContentShare.Domain.Enums;

namespace ContentShare.Application.DTOs.Content;

public class ContentUpdateDto
{
    public string Title { get; set; } = default!;
    public string? Description { get; set; }
    public MediaCategory Category { get; set; }
    public string ThumbnailUrl { get; set; } = default!;
    public string ContentUrl { get; set; } = default!;
}
