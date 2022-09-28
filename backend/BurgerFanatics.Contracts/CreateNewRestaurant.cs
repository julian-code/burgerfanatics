namespace BurgerFanatics.Contracts;

public record CreateNewRestaurant(
    string Name,
    string Description,
    Guid LocationId,
    Guid AdministratorId,
    List<OpeningHourViewModel>? OpeningHours);