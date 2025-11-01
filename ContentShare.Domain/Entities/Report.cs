using ContentShare.Domain.Common;
using ContentShare.Domain.Enums;

namespace ContentShare.Domain.Entities;

public class Report : BaseEntity
{
    public Guid RatingId { get; set; }
    public Guid ReporterUserId { get; set; }

    public ReportReason Reason { get; set; }
    public string? Note { get; set; }

    public Rating? Rating { get; set; }
}
