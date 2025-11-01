using ContentShare.Application.DTOs.Reports;
using ContentShare.Domain.Enums;
using Swashbuckle.AspNetCore.Filters;

namespace ContentShare.API.SwaggerExamples.Reports;

public sealed class ReportCreateDtoExample : IExamplesProvider<ReportCreateDto>
{
    public ReportCreateDto GetExamples() => new()
    {
        Reason = ReportReason.HateSpeech,
        Note = "Contains slurs."
    };
}
