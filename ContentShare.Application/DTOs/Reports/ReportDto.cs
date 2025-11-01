using ContentShare.Domain.Enums;

namespace ContentShare.Application.DTOs.Reports;

public sealed class ReportDto
{
    public Guid Id { get; set; }
    public Guid RatingId { get; set; }
    public Guid ReporterUserId { get; set; }
    public ReportReason Reason { get; set; }
    public string? Note { get; set; }
    public DateTime CreatedAt { get; set; }
}
