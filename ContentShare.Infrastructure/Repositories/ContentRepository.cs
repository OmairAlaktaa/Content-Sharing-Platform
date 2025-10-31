using ContentShare.Application.Interfaces.Repositories;
using ContentShare.Domain.Entities;
using ContentShare.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ContentShare.Infrastructure.Repositories;

public class ContentRepository(AppDbContext db) : IContentRepository
{
    private readonly AppDbContext _db = db;

    public Task<MediaContent?> GetAsync(Guid id, CancellationToken ct = default) =>
        _db.MediaContents.Include(x => x.Ratings).FirstOrDefaultAsync(x => x.Id == id, ct)!;

    public IQueryable<MediaContent> Query() =>
        _db.MediaContents.AsNoTracking().Include(x => x.Ratings);

    public Task AddAsync(MediaContent entity, CancellationToken ct = default) =>
        _db.MediaContents.AddAsync(entity, ct).AsTask();

    public Task UpdateAsync(MediaContent entity, CancellationToken ct = default)
    { _db.MediaContents.Update(entity); return Task.CompletedTask; }

    public Task DeleteAsync(MediaContent entity, CancellationToken ct = default)
    { _db.MediaContents.Remove(entity); return Task.CompletedTask; }
}
