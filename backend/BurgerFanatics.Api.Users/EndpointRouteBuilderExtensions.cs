using BurgerFanatics.Contracts;
using BurgerFanatics.Infrastructure;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace CustomerPortal.Api.Users;

public static class EndpointRouteBuilderExtensions
{
    /// <summary>
    /// <para>This adds the possibility of fetching new users. This includes:</para>
    /// <para>GET /api/users/{id} which fetches the user by ID</para>
    /// </summary>
    /// <param name="endpointRouteBuilder"></param>
    /// <returns></returns>
    public static IEndpointRouteBuilder AddUserFeatures(this IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder.MapGet("/api/users/{id}", async (BurgerFanaticsDbContext context, Guid id) =>
                await context.Users.FirstOrDefaultAsync(x => x.UserId == id) is { } user
                    ? Results.Ok(new UserViewModel(user.UserId, user.Username))
                    : Results.NotFound())
            .WithName("GetUserById")
            .Produces<UserViewModel>();

        return endpointRouteBuilder;
    }
}