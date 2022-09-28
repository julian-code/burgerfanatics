using NodaTime;

namespace BurgerFanatics.Domain.Domain.Models;

public sealed class Review
{
    public Review()
    {
        FileAttachments = new HashSet<FileAttachment>();
    }

    public Guid ReviewId { get; set; }
    public string Description { get; set; } = null!;
    public int RatingTaste { get; set; }
    public int RatingVisual { get; set; }
    public int RatingTexture { get; set; }
    public Instant CreatedUtc { get; set; }
    public Instant UpdatedUtc { get; set; }
    public Guid UserId { get; set; }
    public Guid RestaurantId { get; set; }

    public Restaurant Restaurant { get; set; } = null!;
    public User User { get; set; } = null!;
    public ICollection<FileAttachment> FileAttachments { get; set; }
}