using ContentShare.Application.DTOs.Common;
using ContentShare.Application.DTOs.Content;
using ContentShare.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ContentShare.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContentsController : ControllerBase
{
    private readonly IContentService _svc;
    public ContentsController(IContentService svc) => _svc = svc;

    [HttpGet]
    public async Task<ActionResult<PagedResult<ContentDto>>> Get([FromQuery] PagedRequest req, CancellationToken ct)
        => Ok(await _svc.GetAsync(req, ct));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ContentDto>> GetById(Guid id, CancellationToken ct)
    {
        var item = await _svc.GetByIdAsync(id, ct);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult> Create([FromBody] ContentCreateDto dto, CancellationToken ct)
    {
        var id = await _svc.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetById), new { id }, null);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult> Update(Guid id, [FromBody] ContentUpdateDto dto, CancellationToken ct)
    {
        await _svc.UpdateAsync(id, dto, ct);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _svc.DeleteAsync(id, ct);
        return NoContent();
    }
}
