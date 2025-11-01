using ContentShare.Application.DTOs.Content;
using ContentShare.Domain.Enums;
using Swashbuckle.AspNetCore.Filters;

namespace ContentShare.API.SwaggerExamples.Content;

public sealed class ContentCreateDtoExample : IExamplesProvider<ContentCreateDto>
{
    public ContentCreateDto GetExamples() => new()
    {
        Title = "Epic Trailer",
        Description = "Teaser trailer for the upcoming game.",
        Category = MediaCategory.Video,
        ThumbnailUrl = "https://cdn.example.com/thumbs/trailer.jpg",
        ContentUrl = "https://cdn.example.com/videos/trailer.mp4"
    };
}