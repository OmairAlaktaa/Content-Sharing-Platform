using System.Security.Claims;
using ContentShare.Application.DTOs.Rating;
using ContentShare.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ContentShare.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RatingsController(IRatingService ratings) : ControllerBase
{
    private readonly IRatingService _ratings = ratings;

    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(RatingDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RatingDto>> AddOrUpdate([FromBody] RatingCreateDto dto, CancellationToken ct)
    {
        var sub = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        if (string.IsNullOrWhiteSpace(sub)) return Unauthorized();

        var userId = Guid.Parse(sub);
        var result = await _ratings.AddOrUpdateAsync(userId, dto, ct);
        return Ok(result);
    }

    [HttpGet("content/{contentId:guid}")]
    [Authorize]
    [ProducesResponseType(typeof(IEnumerable<RatingDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<RatingDto>>> GetByContent(Guid contentId, CancellationToken ct)
    {
        var list = await _ratings.GetByContentAsync(contentId, ct);
        return Ok(list);
    }

    [HttpGet("content/{contentId:guid}/average")]
    [Authorize]
    [ProducesResponseType(typeof(double), StatusCodes.Status200OK)]
    public async Task<ActionResult<double>> GetAverage(Guid contentId, CancellationToken ct)
    {
        var avg = await _ratings.GetAverageAsync(contentId, ct);
        return Ok(avg);
    }
}
