using NetTopologySuite.Geometries;

namespace BurgerFanatics.Domain.Domain.Models;

public sealed class Address
{
    public Address()
    {
        Restaurants = new HashSet<Restaurant>();
    }

    public Guid AddressId { get; set; }
    public string Text { get; set; } = null!;
    public Point Location { get; set; } = null!;

    public ICollection<Restaurant> Restaurants { get; set; }
}