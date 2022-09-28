using BurgerFanatics.Domain.Domain.Models;

namespace BurgerFanatics.Domain.Interfaces;

public interface ILocationProvider
{
    Task<IReadOnlyCollection<Address>> SearchAddresses(string query);
    Task<Address?> GetAddress(Guid id);
}