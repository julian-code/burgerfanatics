using BurgerFanatics.Domain.Domain.Models;
using BurgerFanatics.Domain.Interfaces;
using BurgerFanatics.Infrastructure;

using Microsoft.EntityFrameworkCore;

namespace BurgerFanatics.Api.Locations.LocationProviders;

public class DatabaseLocationProvider : ILocationProvider
{
    private readonly ILocationProvider _locationProvider;
    private readonly BurgerFanaticsDbContext _context;

    public DatabaseLocationProvider(ILocationProvider locationProvider, BurgerFanaticsDbContext context)
    {
        _locationProvider = locationProvider;
        _context = context;
    }
    public Task<IReadOnlyCollection<Address>> SearchAddresses(string query)
    {
        return _locationProvider.SearchAddresses(query);
    }

    public async Task<Address?> GetAddress(Guid id)
    {
        var address = await _context.Addresses.FirstOrDefaultAsync(x => x.AddressId == id);
        if (address is not null)
        {
            return address;
        }

        var fetchedAddress = await _locationProvider.GetAddress(id);
        if (fetchedAddress is null)
        {
            return address;
        }

        address = fetchedAddress;
        await _context.Addresses.AddAsync(address);
        await _context.SaveChangesAsync();

        return address;
    }
}