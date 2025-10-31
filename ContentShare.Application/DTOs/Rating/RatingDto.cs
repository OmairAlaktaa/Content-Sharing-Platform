namespace ContentShare.Application.DTOs.Rating;

public class RatingDto
{
    public Guid Id { get; set; }
    public Guid MediaContentId { get; set; }
    public Guid UserId { get; set; }
    public int Score { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; }
}
