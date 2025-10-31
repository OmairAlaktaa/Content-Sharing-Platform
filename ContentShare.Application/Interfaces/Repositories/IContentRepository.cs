using ContentShare.Domain.Entities;

namespace ContentShare.Application.Interfaces.Repositories;

public interface IContentRepository
{
    Task<MediaContent?> GetAsync(Guid id, CancellationToken ct = default);
    IQueryable<MediaContent> Query();
    Task AddAsync(MediaContent entity, CancellationToken ct = default);
    Task UpdateAsync(MediaContent entity, CancellationToken ct = default);
    Task DeleteAsync(MediaContent entity, CancellationToken ct = default);
}
