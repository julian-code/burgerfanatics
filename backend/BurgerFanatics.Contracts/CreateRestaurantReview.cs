namespace BurgerFanatics.Contracts;

public record CreateRestaurantReview(
    string Description,
    int RatingTaste,
    int RatingVisual,
    int RatingTexture,
    Guid UserId,
    HashSet<Guid> AttachmentIds);