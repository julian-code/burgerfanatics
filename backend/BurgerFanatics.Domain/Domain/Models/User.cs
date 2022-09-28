namespace BurgerFanatics.Domain.Domain.Models;

public sealed class User
{
    public User()
    {
        Restaurants = new HashSet<Restaurant>();
        Reviews = new HashSet<Review>();
    }

    public Guid UserId { get; set; }
    public string Username { get; set; } = null!;

    public ICollection<Restaurant> Restaurants { get; set; }
    public ICollection<Review> Reviews { get; set; }
}