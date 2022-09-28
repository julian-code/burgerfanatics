using NodaTime;

namespace BurgerFanatics.Domain.Domain.Models;

public sealed class Restaurant
{
    public Restaurant()
    {
        Reviews = new HashSet<Review>();
        WorkCalendars = new HashSet<WorkCalendar>();
    }

    public Guid RestaurantId { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public Instant CreatedUtc { get; set; }
    public Instant UpdatedUtc { get; set; }
    public Guid AdministratorId { get; set; }
    public Guid AddressId { get; set; }

    public Address Address { get; set; } = null!;
    public User Administrator { get; set; } = null!;
    public ICollection<Review> Reviews { get; set; }
    public ICollection<WorkCalendar> WorkCalendars { get; set; }
}