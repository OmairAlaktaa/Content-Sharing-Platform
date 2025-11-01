using ContentShare.Domain.Enums;

namespace ContentShare.Application.DTOs.Reports;

public sealed class ReportCreateDto
{
    public ReportReason Reason { get; set; }
    public string? Note { get; set; }
}
