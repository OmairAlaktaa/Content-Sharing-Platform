using ContentShare.API.SwaggerExamples.Reports;
using ContentShare.Application.DTOs.Reports;
using ContentShare.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;
using System.Security.Claims;

namespace ContentShare.API.Controllers;

[ApiController]
[Route("api/reviews/{ratingId:guid}/reports")]
public sealed class ReportsController(IReportService reportService) : ControllerBase
{
    [HttpPost]
    [Authorize]
    [SwaggerRequestExample(typeof(ReportCreateDto), typeof(ReportCreateDtoExample))]
    public async Task<IActionResult> Create([FromRoute] Guid ratingId, [FromBody] ReportCreateDto dto, CancellationToken ct)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        await reportService.ReportAsync(ratingId, Guid.Parse(userId), dto.Reason, dto.Note, ct);
        return NoContent();
    }
}
