using System.Net.Http.Json;
using System.Text.Json.Serialization;

using BurgerFanatics.Domain.Domain.Models;
using BurgerFanatics.Domain.Interfaces;

using NetTopologySuite.Geometries;

namespace BurgerFanatics.Api.Locations.LocationProviders;

internal class DAWALocationProvider : ILocationProvider
{
    private readonly HttpClient _client;

    public DAWALocationProvider(HttpClient client)
    {
        _client = client;
    }

    public async Task<IReadOnlyCollection<Address>> SearchAddresses(string query) =>
        await _client.GetFromJsonAsync<List<DawaAutocompleteResponse>>($"adresser/autocomplete/?q={query}") is { } response
            ? response.Select(x => new Address { AddressId = x.Address.Id, Location = new Point(x.Address.X, x.Address.Y), Text = x.Text }).ToList()
            : new List<Address>();

    public async Task<Address?> GetAddress(Guid id) =>
        await _client.GetFromJsonAsync<DawaAddressByIdResponse>($"adresser/{id}?format=geojson") is { } response
            ? new Address
            {
                AddressId = response.Properties.Id,
                Location = new Point(response.Geometry.Coordinates[0], response.Geometry.Coordinates[1]),
                Text = response.Properties.Text
            }
            : null;


    public record DAWAAddress(
        [property: JsonPropertyName("id")] Guid Id,
        [property: JsonPropertyName("x")] double X,
        [property: JsonPropertyName("y")] double Y
    );

    public record DawaAutocompleteResponse(
        [property: JsonPropertyName("tekst")] string Text,
        [property: JsonPropertyName("adresse")] DAWAAddress Address
    );

    public record Geometry(
        [property: JsonPropertyName("coordinates")] IReadOnlyList<double> Coordinates
    );

    public record Properties(
        [property: JsonPropertyName("id")] Guid Id,
        [property: JsonPropertyName("betegnelse")] string Text
    );

    public record DawaAddressByIdResponse(
        [property: JsonPropertyName("geometry")] Geometry Geometry,
        [property: JsonPropertyName("properties")] Properties Properties
    );

}