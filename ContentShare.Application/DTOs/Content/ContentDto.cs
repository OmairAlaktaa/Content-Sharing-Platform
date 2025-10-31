using System.Text.Json.Serialization;
using ContentShare.Domain.Enums;

namespace ContentShare.Application.DTOs.Content;

public class ContentDto
{
    [JsonPropertyName("media_id")]
    public Guid Id { get; init; }

    [JsonPropertyName("title")]
    public string Title { get; init; } = default!;

    [JsonPropertyName("description")]
    public string? Description { get; init; }

    [JsonPropertyName("category")]
    public MediaCategory Category { get; init; }

    [JsonPropertyName("thumbnail_url")]
    public string ThumbnailUrl { get; init; } = default!;

    [JsonPropertyName("content_url")]
    public string ContentUrl { get; init; } = default!;

    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; init; }

    [JsonPropertyName("average_rating")]
    public double AverageRating { get; init; }

    [JsonPropertyName("ratings_count")]
    public int RatingsCount { get; init; }
}
