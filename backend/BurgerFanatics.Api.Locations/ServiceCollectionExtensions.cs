using BurgerFanatics.Api.Locations.LocationProviders;
using BurgerFanatics.Domain.Interfaces;

using Microsoft.Extensions.DependencyInjection;

namespace BurgerFanatics.Api.Locations;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// We need to provide a way of fetching the current location of a user, which our
    /// location providers make sure.
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddLocationProviders(this IServiceCollection services)
    {
        // We utilize HttpClientFactory since usage of HttpClient is quite cumbersome. This is also recommended
        // practice by ASP.NET Core.
        services.AddHttpClient<ILocationProvider, DAWALocationProvider>(options =>
            options.BaseAddress = new Uri("https://api.dataforsyningen.dk"));

        return services;
    }
}