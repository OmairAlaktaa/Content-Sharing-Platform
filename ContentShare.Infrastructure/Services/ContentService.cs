using ContentShare.Application.DTOs.Common;
using ContentShare.Application.DTOs.Content;
using ContentShare.Application.Interfaces.Repositories;
using ContentShare.Application.Interfaces.Services;
using ContentShare.Domain.Entities;
using ContentShare.Domain.Enums;
using ContentShare.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ContentShare.Infrastructure.Services;

public class ContentService(IContentRepository repo, AppDbContext db) : IContentService
{
    private readonly IContentRepository _repo = repo;
    private readonly AppDbContext _db = db;

    public async Task<Guid> CreateAsync(ContentCreateDto dto, CancellationToken ct = default)
    {
        var entity = new MediaContent
        {
            Title = dto.Title,
            Description = dto.Description,
            Category = dto.Category,
            ThumbnailUrl = dto.ThumbnailUrl,
            ContentUrl = dto.ContentUrl
        };
        await _repo.AddAsync(entity, ct);
        await _db.SaveChangesAsync(ct);
        return entity.Id;
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _repo.GetAsync(id, ct) ?? throw new KeyNotFoundException("Content not found");
        await _repo.DeleteAsync(entity, ct);
        await _db.SaveChangesAsync(ct);
    }

    public async Task<PagedResult<ContentDto>> GetAsync(PagedRequest request, CancellationToken ct = default)
    {
        var q = _repo.Query();

        if (!string.IsNullOrWhiteSpace(request.Search))
            q = q.Where(x => x.Title.Contains(request.Search));

        if (!string.IsNullOrWhiteSpace(request.Category) &&
            Enum.TryParse<MediaCategory>(request.Category, true, out var cat))
            q = q.Where(x => x.Category == cat);

        q = request.Sort?.ToLower() switch
        {
            "title" => q.OrderBy(x => x.Title),
            "-title" => q.OrderByDescending(x => x.Title),
            "created_at" => q.OrderBy(x => x.CreatedAt),
            "-created_at" => q.OrderByDescending(x => x.CreatedAt),
            _ => q.OrderByDescending(x => x.CreatedAt)
        };

        var total = await q.CountAsync(ct);
        var items = await q.Skip((request.Page - 1) * request.PageSize)
                           .Take(request.PageSize)
                           .Select(x => new ContentDto
                           {
                               Id = x.Id,
                               Title = x.Title,
                               Description = x.Description,
                               Category = x.Category,
                               ThumbnailUrl = x.ThumbnailUrl,
                               ContentUrl = x.ContentUrl,
                               CreatedAt = x.CreatedAt,
                               AverageRating = x.Ratings.Any() ? x.Ratings.Average(r => r.Score) : 0.0,
                               RatingsCount = x.Ratings.Count
                           })
                           .ToListAsync(ct);

        return new PagedResult<ContentDto>(items, total, request.Page, request.PageSize);
    }

    public async Task<ContentDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var x = await _repo.GetAsync(id, ct);
        if (x is null) return null;

        return new ContentDto
        {
            Id = x.Id,
            Title = x.Title,
            Description = x.Description,
            Category = x.Category,
            ThumbnailUrl = x.ThumbnailUrl,
            ContentUrl = x.ContentUrl,
            CreatedAt = x.CreatedAt,
            AverageRating = x.Ratings.Any() ? x.Ratings.Average(r => r.Score) : 0.0,
            RatingsCount = x.Ratings.Count
        };
    }

    public async Task UpdateAsync(Guid id, ContentUpdateDto dto, CancellationToken ct = default)
    {
        var entity = await _repo.GetAsync(id, ct) ?? throw new KeyNotFoundException("Content not found");

        entity.Title = dto.Title;
        entity.Description = dto.Description;
        entity.Category = dto.Category;
        entity.ThumbnailUrl = dto.ThumbnailUrl;
        entity.ContentUrl = dto.ContentUrl;

        await _repo.UpdateAsync(entity, ct);
        await _db.SaveChangesAsync(ct);
    }
}
