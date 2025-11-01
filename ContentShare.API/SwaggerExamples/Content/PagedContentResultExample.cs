using System.Collections.Generic;
using ContentShare.Application.DTOs.Common;
using ContentShare.Application.DTOs.Content;
using Swashbuckle.AspNetCore.Filters;

namespace ContentShare.API.SwaggerExamples.Content;

public sealed class PagedContentResultExample : IExamplesProvider<PagedResult<ContentDto>>
{
    public PagedResult<ContentDto> GetExamples()
    {
        var item = new ContentDtoExample().GetExamples();
        var items = new List<ContentDto> { item };

        return new PagedResult<ContentDto>(items, total: 1, page: 1, pageSize: 10);
    }
}
