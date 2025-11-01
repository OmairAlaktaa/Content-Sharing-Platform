using ContentShare.Domain.Enums;

namespace ContentShare.Application.Interfaces.Services;

public interface IReportService
{
    Task ReportAsync(Guid ratingId, Guid reporterUserId, ReportReason reason, string? note, CancellationToken ct = default);
}
