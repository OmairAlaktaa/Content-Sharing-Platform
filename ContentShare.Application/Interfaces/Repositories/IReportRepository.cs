using ContentShare.Domain.Entities;

namespace ContentShare.Application.Interfaces.Repositories;

public interface IReportRepository
{
    Task<bool> HasUserReportedAsync(Guid ratingId, Guid userId, CancellationToken ct = default);
    Task AddAsync(Report report, CancellationToken ct = default);
    Task<int> CountByRatingAsync(Guid ratingId, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
