using NodaTime;

namespace BurgerFanatics.Domain.Domain.Models;

public class WorkCalendar
{
    public Guid RestaurantId { get; set; }
    public string WeekDay { get; set; } = null!;
    public LocalTime? OpeningHour { get; set; }
    public LocalTime? ClosingHour { get; set; }

    public virtual Restaurant Restaurant { get; set; } = null!;
}