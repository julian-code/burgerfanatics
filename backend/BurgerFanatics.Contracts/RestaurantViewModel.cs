using NodaTime;

namespace BurgerFanatics.Contracts;

public record RestaurantViewModel(
    Guid RestaurantId,
    string Name,
    string Description,
    Instant CreatedUtc,
    LocationViewModel Location,
    double? AverageRating,
    HashSet<OpeningHourViewModel> OpeningHours,
    double? DistanceMeters = null);

public record OpeningHourViewModel(string Day, LocalTime? OpeningHour, LocalTime? ClosingHour);