using ContentShare.Application.Interfaces.Repositories;
using ContentShare.Domain.Entities;
using ContentShare.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ContentShare.Infrastructure.Repositories;

public sealed class ReportRepository(AppDbContext db) : IReportRepository
{
    public Task<bool> HasUserReportedAsync(Guid ratingId, Guid userId, CancellationToken ct = default)
        => db.Reports.AnyAsync(r => r.RatingId == ratingId && r.ReporterUserId == userId, ct);

    public async Task AddAsync(Report report, CancellationToken ct = default)
        => await db.Reports.AddAsync(report, ct);

    public Task<int> CountByRatingAsync(Guid ratingId, CancellationToken ct = default)
        => db.Reports.CountAsync(r => r.RatingId == ratingId, ct);

    public Task SaveChangesAsync(CancellationToken ct = default)
        => db.SaveChangesAsync(ct);
}
