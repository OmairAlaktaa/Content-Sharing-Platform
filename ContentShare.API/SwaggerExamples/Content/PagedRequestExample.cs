using ContentShare.Application.DTOs.Common;
using Swashbuckle.AspNetCore.Filters;

namespace ContentShare.API.SwaggerExamples.Content;

public sealed class PagedRequestExample : IExamplesProvider<PagedRequest>
{
    public PagedRequest GetExamples() => new()
    {
        Page = 1,
        PageSize = 10,
        Sort = "createdAt desc",
        Search = "trailer"
    };
}