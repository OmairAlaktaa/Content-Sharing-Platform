using ContentShare.Application.DTOs.Common;
using ContentShare.Application.DTOs.Content;

namespace ContentShare.Application.Interfaces.Services;

public interface IContentService
{
    Task<PagedResult<ContentDto>> GetAsync(PagedRequest request, CancellationToken ct = default);
    Task<ContentDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Guid> CreateAsync(ContentCreateDto dto, CancellationToken ct = default);
    Task UpdateAsync(Guid id, ContentUpdateDto dto, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}
