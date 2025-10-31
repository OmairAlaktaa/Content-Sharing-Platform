using ContentShare.Domain.Common;

namespace ContentShare.Domain.Entities;

public class Rating : BaseEntity
{
    public Guid MediaContentId { get; set; }
    public Guid UserId { get; set; }
    public int Score { get; set; }
}
