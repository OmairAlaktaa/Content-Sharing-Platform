using ContentShare.Application.DTOs.Content;
using ContentShare.Domain.Enums;
using Swashbuckle.AspNetCore.Filters;

namespace ContentShare.API.SwaggerExamples.Content;

public sealed class ContentUpdateDtoExample : IExamplesProvider<ContentUpdateDto>
{
    public ContentUpdateDto GetExamples() => new()
    {
        Title = "Epic Trailer (Updated)",
        Description = "Updated description",
        Category = MediaCategory.Video,
        ThumbnailUrl = "https://cdn.example.com/thumbs/trailer-v2.jpg",
        ContentUrl = "https://cdn.example.com/videos/trailer-v2.mp4"
    };
}