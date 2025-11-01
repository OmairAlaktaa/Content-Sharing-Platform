using ContentShare.Application.DTOs.Content;
using ContentShare.Domain.Enums;
using Swashbuckle.AspNetCore.Filters;

namespace ContentShare.API.SwaggerExamples.Content;

public sealed class ContentDtoExample : IExamplesProvider<ContentDto>
{
    public ContentDto GetExamples() => new()
    {
        Id = Guid.Parse("aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee"),
        Title = "Epic Trailer",
        Description = "Teaser trailer for the upcoming game.",
        Category = MediaCategory.Video,
        ThumbnailUrl = "https://cdn.example.com/thumbs/trailer.jpg",
        ContentUrl = "https://cdn.example.com/videos/trailer.mp4",
        CreatedAt = DateTime.UtcNow.AddDays(-3),
        AverageRating = 4.3
    };
}