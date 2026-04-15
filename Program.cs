using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MinimalAPI.Domain.Interfaces;
using MinimalAPI.Domain.ModelViews;
using MinimalAPI.Domain.Services;
using MinimalAPI.Domain.DTOs;
using MinimalAPI.Infra.DB;
using MinimalAPI.Domain.Entities;

#region Builder
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IVehicleService, VehicleService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<DBContext>(options =>
{
    options.UseMySql(
        builder.Configuration.GetConnectionString("mysql"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("mysql"))
    );
});


var app = builder.Build();
#endregion

#region Home
app.MapGet("/", () =>
{
    return Results.Json(new Home());
}).WithTags("Home");

#endregion

#region Admin
app.MapPost("/admin/login", ([FromBody] LoginDTO loginDTO, IAdminService adminService) =>
{
   var admin = adminService.Login(loginDTO);
   if (admin == null)
   {
       return Results.Unauthorized();
   }
   return Results.Ok(admin);
}).WithTags("Admin");

#endregion

#region Vehicles
app.MapPost("/vehicles", ([FromBody] VehicleDTO vehicleDTO, IVehicleService vehicleService) =>
{
    var vehicle = new Vehicle
    {
        Name = vehicleDTO.Name,
        Brand = vehicleDTO.Brand,
        Year = vehicleDTO.Year,
        Model = vehicleDTO.Model
    };
    vehicleService.CreateVehicle(vehicle);
    return Results.Created($"/vehicles/{vehicle.Id}", vehicle);
}).WithTags("Vehicles");

app.MapGet("/vehicles", (IVehicleService vehicleService, [FromQuery] int? page = null) =>
{
    var vehicles = vehicleService.GetAllVehicles(page ?? 1);
    return Results.Ok(vehicles);
}).WithTags("Vehicles");

app.MapGet("/vehicles/{id}", ([FromRoute] int id, IVehicleService vehicleService) =>
{
    var vehicle = vehicleService.GetVehicleById(id);
    if (vehicle == null)
    {
        return Results.NotFound();
    }
    return Results.Ok(vehicle);
}).WithTags("Vehicles");

app.MapPut("/vehicles/{id}", ([FromRoute] int id, [FromBody] VehicleDTO vehicleDTO, IVehicleService vehicleService) =>
{
    var existingVehicle = vehicleService.GetVehicleById(id);
    if (existingVehicle == null)
    {
        return Results.NotFound();
    }

    existingVehicle.Name = vehicleDTO.Name;
    existingVehicle.Brand = vehicleDTO.Brand;
    existingVehicle.Year = vehicleDTO.Year;
    existingVehicle.Model = vehicleDTO.Model;

    vehicleService.UpdateVehicle(existingVehicle);
    return Results.Ok(existingVehicle);
}).WithTags("Vehicles");

app.MapDelete("/vehicles/{id}", ([FromRoute] int id, IVehicleService vehicleService) =>
{
    var vehicle = vehicleService.GetVehicleById(id);
    if (vehicle == null)
    {
        return Results.NotFound();
    }

    vehicleService.DeleteVehicle(vehicle);
    return Results.NoContent();
}).WithTags("Vehicles");
#endregion

#region App
app.UseSwagger();
app.UseSwaggerUI();

app.Run();
#endregion
