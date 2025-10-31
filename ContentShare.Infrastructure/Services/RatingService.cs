using ContentShare.Application.DTOs.Rating;
using ContentShare.Application.Interfaces.Repositories;
using ContentShare.Application.Interfaces.Services;
using ContentShare.Domain.Entities;

namespace ContentShare.Infrastructure.Services;

public class RatingService(IRatingRepository ratings, IContentRepository contents) : IRatingService
{
    private readonly IRatingRepository _ratings = ratings;
    private readonly IContentRepository _contents = contents;

    public async Task<RatingDto> AddOrUpdateAsync(Guid userId, RatingCreateDto dto, CancellationToken ct = default)
    {
        var content = await _contents.GetAsync(dto.MediaContentId, ct);
        if (content is null) throw new KeyNotFoundException("Content not found.");

        var existing = await _ratings.GetByUserAndContentAsync(userId, dto.MediaContentId, ct);

        if (existing is null)
        {
            var rating = new Rating
            {
                MediaContentId = dto.MediaContentId,
                UserId = userId,
                Score = dto.Score,
                Comment = dto.Comment
            };
            await _ratings.AddAsync(rating, ct);

            return new RatingDto
            {
                Id = rating.Id,
                MediaContentId = rating.MediaContentId,
                UserId = rating.UserId,
                Score = rating.Score,
                Comment = rating.Comment,
                CreatedAt = rating.CreatedAt
            };
        }
        else
        {
            existing.Score = dto.Score;
            existing.Comment = dto.Comment;
            await _ratings.UpdateAsync(existing, ct);

            return new RatingDto
            {
                Id = existing.Id,
                MediaContentId = existing.MediaContentId,
                UserId = existing.UserId,
                Score = existing.Score,
                Comment = existing.Comment,
                CreatedAt = existing.CreatedAt
            };
        }
    }

    public async Task<IReadOnlyList<RatingDto>> GetByContentAsync(Guid contentId, CancellationToken ct = default)
    {
        var content = await _contents.GetAsync(contentId, ct);
        if (content is null) throw new KeyNotFoundException("Content not found.");

        var list = await _ratings.GetByContentAsync(contentId, ct);
        return list.Select(r => new RatingDto
        {
            Id = r.Id,
            MediaContentId = r.MediaContentId,
            UserId = r.UserId,
            Score = r.Score,
            Comment = r.Comment,
            CreatedAt = r.CreatedAt
        }).ToList();
    }

    public Task<double> GetAverageAsync(Guid contentId, CancellationToken ct = default)
        => _ratings.GetAverageForContentAsync(contentId, ct);
}
