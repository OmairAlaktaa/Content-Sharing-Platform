using ContentShare.Application.DTOs.Rating;
using Swashbuckle.AspNetCore.Filters;

namespace ContentShare.API.SwaggerExamples.Ratings;

public sealed class RatingDtoExample : IExamplesProvider<RatingDto>
{
    public RatingDto GetExamples() => new()
    {
        Id = Guid.Parse("bbbbbbbb-cccc-dddd-eeee-ffffffffffff"),
        MediaContentId = Guid.Parse("aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee"),
        UserId = Guid.Parse("11111111-2222-3333-4444-555555555555"),
        Score = 5,
        Comment = "Amazing work!",
        CreatedAt = DateTime.UtcNow.AddHours(-2)
    };
}