using ContentShare.Domain.Common;

namespace ContentShare.Domain.Entities;

public class Rating : BaseEntity
{
    public Guid MediaContentId { get; set; }
    public Guid UserId { get; set; }
    public int Score { get; set; }
    public string? Comment { get; set; }
    public int ReportsCount { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTimeOffset? DeletedAt { get; private set; }

    public const int AutoDeleteThreshold = 10;

    public void AddReportAndMaybeDelete()
    {
        ReportsCount++;
        if (!IsDeleted && ReportsCount >= AutoDeleteThreshold)
        {
            IsDeleted = true;
            DeletedAt = DateTimeOffset.UtcNow;
        }
    }
}