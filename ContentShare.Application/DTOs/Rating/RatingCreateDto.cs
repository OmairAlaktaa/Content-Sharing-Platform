namespace ContentShare.Application.DTOs.Rating;

public class RatingCreateDto
{
    public Guid MediaContentId { get; set; }
    public int Score { get; set; }
    public string? Comment { get; set; }
}
