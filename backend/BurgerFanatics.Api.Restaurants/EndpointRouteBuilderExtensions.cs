using System.Net;

using BurgerFanatics.Contracts;
using BurgerFanatics.Domain.Domain.Models;
using BurgerFanatics.Domain.Interfaces;
using BurgerFanatics.Infrastructure;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

using NetTopologySuite.Geometries;

namespace BurgerFanatics.Api.Restaurants;

public static class EndpointRouteBuilderExtensions
{
    /// <summary>
    /// <para>This adds all features regarding restaurants. This includes:</para>
    /// <para>GET /api/restaurants which fetches all restaurants, with the optional parameter of location</para>
    /// <para>POST /api/restaurants which creates a new restaurant</para>
    /// <para>GET /api/restaurants/{id} fetches the restaurant by ID</para>
    /// <para>GET /api/restaurant/{id}/reviews fetches all reviews of the restaurant by ID</para>
    /// <para>POST /api/restaurant/{id}/reviews creates a new review on a restaurant</para>
    /// </summary>
    /// <param name="endpointRouteBuilder"></param>
    /// <returns></returns>
    /// <exception cref="BadHttpRequestException"></exception>
    public static IEndpointRouteBuilder AddRestaurantFeatures(this IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder.MapGet("/api/restaurants",
            async (BurgerFanaticsDbContext context,
                ILocationProvider locationProvider,
                [FromQuery(Name = "l")] Guid? locationId,
                [FromQuery(Name = "x")] double? xCoordinate,
                [FromQuery(Name = "y")] double? yCoordinate) =>
            {
                // We utilize pattern matching to figure out the GPS coordinates.
                var point = (locationId, xCoordinate, yCoordinate) switch
                {
                    ({ }, null, null) => await locationProvider.GetAddress(locationId.Value) is { } address
                        ? address.Location
                        : throw new BadHttpRequestException($"Could not find address with id {locationId}"),
                    (null, { }, { }) => new Point(xCoordinate.Value, yCoordinate.Value),
                    ({ }, { }, { }) => throw new BadHttpRequestException(
                        "Please provide either X and Y coordinates or l as location id",
                        (int)HttpStatusCode.BadRequest),
                    _ => null
                };

                var query = context.Restaurants.AsQueryable().AsNoTracking().Take(10);
                RestaurantViewModel[] result;
                if (point is not null)
                {
                    result = await query
                        .Select(x => 
                            new RestaurantViewModel(
                                x.RestaurantId, 
                                x.Name, 
                                x.Description,
                                x.CreatedUtc,
                                new LocationViewModel(
                                    x.Address.AddressId, 
                                    x.Address.Location, 
                                    x.Address.Text), 
                                x.Reviews.Count > 0
                                ? x.Reviews.Sum(y => y.RatingTaste + y.RatingTexture + y.RatingVisual) / x.Reviews.Count / 3
                                : null, 
                                x.WorkCalendars.Select(y => 
                                        new OpeningHourViewModel(
                                            y.WeekDay.ToString(),
                                            y.OpeningHour,
                                            y.ClosingHour))
                                    .ToHashSet(),
                            x.Address.Location.Distance(point)))
                        .ToArrayAsync();
                }
                else
                {
                    result = await query.Select(x =>
                            new RestaurantViewModel(
                                x.RestaurantId,
                                x.Name,
                                x.Description,
                                x.CreatedUtc,
                                new LocationViewModel(
                                    x.Address.AddressId,
                                    x.Address.Location,
                                    x.Address.Text),
                                x.Reviews.Count > 0
                                    ? x.Reviews.Sum(y => y.RatingTaste + y.RatingTexture + y.RatingVisual) / x.Reviews.Count / 3
                                    : null, x.WorkCalendars.Select(y => new OpeningHourViewModel(
                                        y.WeekDay.ToString(),
                                        y.OpeningHour,
                                        y.ClosingHour))
                                    .ToHashSet(),
                                null))
                        .ToArrayAsync();
                }

                return point is null
                    ? result.OrderBy(x => x.DistanceMeters)
                        .ThenByDescending(x => x.AverageRating)
                    : result.OrderByDescending(x => x.AverageRating);
            });

        endpointRouteBuilder.MapPost("/api/restaurants", async (
                BurgerFanaticsDbContext context, 
                ILocationProvider locationProvider, 
                [FromBody] CreateNewRestaurant request) =>
        {
            var location = await locationProvider.GetAddress(request.LocationId);
            if (location is null)
            {
                return Results.NotFound($"Address with ID {request.LocationId} was not found");
            }

            var administrator = await context.Users.FirstOrDefaultAsync(x => x.UserId == request.AdministratorId);
            if (administrator is null)
            {
                return Results.NotFound($"User with ID {request.AdministratorId} was not found");
            }

            var restaurant = new Restaurant
            {
                Name = request.Name,
                Administrator = administrator,
                Description = request.Description,
                Address = location
            };

            var workCalendar = request.OpeningHours?.Select(x => new WorkCalendar
                               {
                                   ClosingHour = x.ClosingHour,
                                   OpeningHour = x.OpeningHour,
                                   WeekDay = x.Day,
                                   Restaurant = restaurant
                               }) ??
                               new List<WorkCalendar>();

            context.WorkCalendars.AddRange(workCalendar);
            context.Restaurants.Add(restaurant);

            await context.SaveChangesAsync();

            return Results.NoContent();
        })
        .WithName("CreateNewRestaurants");

        endpointRouteBuilder.MapGet("/api/restaurants/{id}", async (BurgerFanaticsDbContext context, Guid id) =>
            {
                var result = await context.Restaurants.Take(1)
                    .Select(x =>
                        new RestaurantViewModel(
                            x.RestaurantId,
                            x.Name,
                            x.Description,
                            x.CreatedUtc,
                            new LocationViewModel(
                                x.Address.AddressId,
                                x.Address.Location,
                                x.Address.Text),
                            x.Reviews.Count > 0
                                ? x.Reviews.Sum(y => y.RatingTaste + y.RatingTexture + y.RatingVisual) / x.Reviews.Count / 3
                                : null, 
                            x.WorkCalendars.Select(y => new OpeningHourViewModel(
                                    y.WeekDay,
                                    y.OpeningHour,
                                    y.ClosingHour))
                                .ToHashSet(),
                            null)).ToArrayAsync();

                return result.FirstOrDefault(x => x.RestaurantId == id) is { } restaurant ? Results.Ok(restaurant) : Results.NotFound();
            })
            .WithName("GetRestaurantById")
        .Produces<RestaurantViewModel>();

        endpointRouteBuilder.MapGet("/api/restaurants/{id}/reviews", async (BurgerFanaticsDbContext context, Guid id) => 
                await context.Reviews.AsNoTracking().Where(x => x.RestaurantId == id).Select(x => 
                    new ReviewViewModel(
                        x.Description, 
                        (x.RatingTaste + x.RatingTexture + x.RatingVisual) / 3,
                        x.RatingTexture,
                        x.RatingVisual,
                        x.RatingTaste)).ToArrayAsync())
        .WithName("GetReviewsByRestaurantId")
        .Produces<ReviewViewModel[]>();

        endpointRouteBuilder.MapPost("api/restaurants/{restaurantId}/reviews", async (
                [FromServices] BurgerFanaticsDbContext context,
                [FromRoute] Guid restaurantId,
                [FromBody] CreateRestaurantReview request) =>
            {
                var review = new Review
                {
                    Description = request.Description,
                    RatingTaste = request.RatingTaste,
                    RatingTexture = request.RatingTexture,
                    RatingVisual = request.RatingVisual,
                    RestaurantId = restaurantId,
                    UserId = request.UserId
                };

                await context.Reviews.AddAsync(review);
                if (request.AttachmentIds.Any())
                {
                    var attachments = await context.FileAttachments
                        .Where(x => request.AttachmentIds.Contains(x.FileAttachmentId))
                        .ToListAsync();
                    foreach (var attachment in attachments)
                    {
                        attachment.Review = review;
                    }
                }

                await context.SaveChangesAsync();
            })
            .WithName("CreateRestaurantReview");

        return endpointRouteBuilder;
    }
}