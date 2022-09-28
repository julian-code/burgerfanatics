using MicroElements.Swashbuckle.NodaTime;

using Microsoft.AspNetCore.Http.Json;
using Microsoft.OpenApi.Models;

using NetTopologySuite.Geometries;
using NetTopologySuite.IO.Converters;

using NodaTime;
using NodaTime.Serialization.SystemTextJson;

namespace BurgerFanatics.Api;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Sets up different JSON converters, so we can convert our strongly typed types to valid JSON.
    /// We fx. use geo json format for our spatial data (NetTopologySuite) and NodaTime for Date and time
    /// types, since BCL DateTime is awful.
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection ConfigureJson(this IServiceCollection services)
    {
        services.Configure<JsonOptions>(x =>
        {
            x.SerializerOptions.Converters.Add(new GeoJsonConverterFactory());
            x.SerializerOptions.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);
        });

        return services;
    }

    /// <summary>
    /// Sets up Swagger which is .NET synonym for Open API spec. This makes it easy
    /// for code generation tools to generate specific client code to interact with our
    /// API, and makes it easy for developers to reason about our different endpoints.
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.MapType<Point>(() => new OpenApiSchema
            {
                Type = "object",
                Properties = new Dictionary<string, OpenApiSchema>
                {
                    {
                        "type",
                        new OpenApiSchema { Type = "string"}
                    },
                    {
                        "coordinates",
                        new OpenApiSchema
                        {
                            Type = "array",
                            MinItems = 2,
                            MaxItems = 2,
                            Items = new OpenApiSchema
                            {
                                Type = "number",
                                Format = "double"
                            }
                        }
                    }

                }
            });
            c.ConfigureForNodaTime();
        });

        return services;
    }
}