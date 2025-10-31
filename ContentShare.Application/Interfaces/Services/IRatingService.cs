using ContentShare.Application.DTOs.Rating;

namespace ContentShare.Application.Interfaces.Services;

public interface IRatingService
{
    Task<RatingDto> AddOrUpdateAsync(Guid userId, RatingCreateDto dto, CancellationToken ct = default);
    Task<IReadOnlyList<RatingDto>> GetByContentAsync(Guid contentId, CancellationToken ct = default);
    Task<double> GetAverageAsync(Guid contentId, CancellationToken ct = default);
}
