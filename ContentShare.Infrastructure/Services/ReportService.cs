using ContentShare.Application.Interfaces.Repositories;
using ContentShare.Application.Interfaces.Services;
using ContentShare.Domain.Entities;
using ContentShare.Domain.Enums;

namespace ContentShare.Infrastructure.Services;

public sealed class ReportService(
    IReportRepository reports,
    IRatingRepository ratings
) : IReportService
{
    private const int AutoDeleteThreshold = 10;

    public async Task ReportAsync(Guid ratingId, Guid reporterUserId, ReportReason reason, string? note, CancellationToken ct = default)
    {
        var rating = await ratings.GetByIdAsync(ratingId, ct);
        if (rating is null)
        {
            throw new KeyNotFoundException("Rating not found.");
        }

        if (await reports.HasUserReportedAsync(ratingId, reporterUserId, ct))
            return;

        var report = new Report
        {
            RatingId = ratingId,
            ReporterUserId = reporterUserId,
            Reason = reason,
            Note = note
        };

        await reports.AddAsync(report, ct);
        await reports.SaveChangesAsync(ct);

        var total = await reports.CountByRatingAsync(ratingId, ct);
        if (total >= AutoDeleteThreshold)
        {
            await ratings.DeleteAsync(ratingId, ct);
            await ratings.SaveChangesAsync(ct);
        }
    }
}
