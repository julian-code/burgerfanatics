using BurgerFanatics.Api;
using BurgerFanatics.Api.Attachments;
using BurgerFanatics.Api.Locations;
using BurgerFanatics.Api.Restaurants;
using BurgerFanatics.Infrastructure;

using CustomerPortal.Api.Users;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureJson();
builder.Services.AddLocationProviders();

// We make sure to have a Open API spec
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwagger();

// We utilize postgres as our database
var connectionString = builder.Configuration["POSTGRES"] ?? "UserID=burger_fanatics_db_user; Password=example; Host=localhost; Port=5432; Database=burger_fanatics_db";
builder.Services.AddDatabase(connectionString);

var app = builder.Build();

// This provides a way to browse our Open API spec.
app.MapSwagger();
app.UseSwaggerUI();

// We use an exception page for easier debugging
app.UseDeveloperExceptionPage();

//We have vertical sliced the API to different libraries, so its easy to move out to its own service.
app.AddRestaurantFeatures();
app.AddAttachmentFeatures();
app.AddUserFeatures();
app.AddLocationFeatures();

app.Run();