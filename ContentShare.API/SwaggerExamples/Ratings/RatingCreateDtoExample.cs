using ContentShare.Application.DTOs.Rating;
using Swashbuckle.AspNetCore.Filters;

namespace ContentShare.API.SwaggerExamples.Ratings;

public sealed class RatingCreateDtoExample : IExamplesProvider<RatingCreateDto>
{
    public RatingCreateDto GetExamples() => new()
    {
        MediaContentId = Guid.Parse("aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee"),
        Score = 5,
        Comment = "Amazing work!"
    };
}