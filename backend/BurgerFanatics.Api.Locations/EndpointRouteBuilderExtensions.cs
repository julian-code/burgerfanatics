using BurgerFanatics.Contracts;
using BurgerFanatics.Domain.Interfaces;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace BurgerFanatics.Api.Locations;

public static class EndpointRouteBuilderExtensions
{
    /// <summary>
    /// <para>This makes it possible to fetch your current location by address. This includes:</para>
    /// <para>GET /api/locations?q={query} which fetches based on the query provided</para>
    /// </summary>
    /// <param name="endpointRouteBuilder"></param>
    /// <returns></returns>
    public static IEndpointRouteBuilder AddLocationFeatures(this IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder.MapGet("/api/locations",
                async (string q, ILocationProvider locationProvider) =>
                    (await locationProvider.SearchAddresses(q)).Take(10).Select(x =>
                        new LocationViewModel(x.AddressId, x.Location, x.Text)).ToArray())
            .WithName("GetLocationsByQuery")
            .Produces<LocationViewModel[]>();

        return endpointRouteBuilder;
    }
}