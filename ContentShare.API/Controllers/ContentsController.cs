using ContentShare.API.SwaggerExamples.Content;
using ContentShare.Application.DTOs.Common;
using ContentShare.Application.DTOs.Content;
using ContentShare.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;

namespace ContentShare.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContentsController(IContentService svc) : ControllerBase
{
    private readonly IContentService _svc = svc;

    [HttpGet]
    [SwaggerRequestExample(typeof(PagedRequest), typeof(PagedRequestExample))]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(PagedContentResultExample))]
    public async Task<ActionResult<PagedResult<ContentDto>>> Get([FromQuery] PagedRequest req, CancellationToken ct)
    {
        return Ok(await _svc.GetAsync(req, ct)); 
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ContentDto>> GetById(Guid id, CancellationToken ct)
    {
        var item = await _svc.GetByIdAsync(id, ct);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpPost]
    [Authorize]
    [SwaggerRequestExample(typeof(ContentCreateDto), typeof(ContentCreateDtoExample))]
    [SwaggerResponseExample(StatusCodes.Status201Created, typeof(ContentDtoExample))]
    public async Task<ActionResult> Create([FromBody] ContentCreateDto dto, CancellationToken ct)
    {
        var id = await _svc.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetById), new { id }, null);
    }

    [HttpPut("{id:guid}")]
    [Authorize]
    [SwaggerRequestExample(typeof(ContentUpdateDto), typeof(ContentUpdateDtoExample))]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(ContentDtoExample))]
    public async Task<ActionResult> Update(Guid id, [FromBody] ContentUpdateDto dto, CancellationToken ct)
    {
        await _svc.UpdateAsync(id, dto, ct);
        return NoContent();
    }

    [Authorize]
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _svc.DeleteAsync(id, ct);
        return NoContent();
    }
}
