using ContentShare.Domain.Entities;

namespace ContentShare.Application.Interfaces.Repositories;

public interface IRatingRepository
{
    Task<Rating?> GetByUserAndContentAsync(Guid userId, Guid mediaContentId, CancellationToken ct = default);
    Task AddAsync(Rating rating, CancellationToken ct = default);
    Task UpdateAsync(Rating rating, CancellationToken ct = default);
    Task<List<Rating>> GetByContentAsync(Guid mediaContentId, CancellationToken ct = default);
    Task<double> GetAverageForContentAsync(Guid mediaContentId, CancellationToken ct = default);
    Task<Rating?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
