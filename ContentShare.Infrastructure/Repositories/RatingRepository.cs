using ContentShare.Application.Interfaces.Repositories;
using ContentShare.Domain.Entities;
using ContentShare.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ContentShare.Infrastructure.Repositories;

public class RatingRepository : IRatingRepository
{
    private readonly AppDbContext _db;

    public RatingRepository(AppDbContext db) => _db = db;

    public Task<Rating?> GetByUserAndContentAsync(Guid userId, Guid mediaContentId, CancellationToken ct = default)
        => _db.Ratings.FirstOrDefaultAsync(r => r.UserId == userId && r.MediaContentId == mediaContentId, ct)!;

    public async Task AddAsync(Rating rating, CancellationToken ct = default)
    {
        await _db.Ratings.AddAsync(rating, ct);
        await _db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Rating rating, CancellationToken ct = default)
    {
        _db.Ratings.Update(rating);
        await _db.SaveChangesAsync(ct);
    }

    public Task<List<Rating>> GetByContentAsync(Guid mediaContentId, CancellationToken ct = default)
        => _db.Ratings
              .Where(r => r.MediaContentId == mediaContentId)
              .OrderByDescending(r => r.CreatedAt)
              .ToListAsync(ct);

    public async Task<double> GetAverageForContentAsync(Guid mediaContentId, CancellationToken ct = default)
        => await _db.Ratings
            .Where(r => r.MediaContentId == mediaContentId)
            .Select(r => (double)r.Score)
            .DefaultIfEmpty(0)
            .AverageAsync(ct);
}
