using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BurgerFanatics.Infrastructure;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// This adds a dependency on Entity Framework Core to easily get an interaction mechanism
    /// to our PostgreSQL instance.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="connectionString"></param>
    /// <returns></returns>
    public static IServiceCollection AddDatabase(this IServiceCollection services, string connectionString)
    {
        // We utilize EF Core and its reverse engineering for databases to scaffold the context. Postgis is used
        // for spatial data in Postgres, which the EFCore provider can translate to NetTopologySuite.
        services.AddDbContext<BurgerFanaticsDbContext>(options =>
            options.UseNpgsql(connectionString, opt => opt.UseNetTopologySuite().UseNodaTime()));

        return services;
    }
}