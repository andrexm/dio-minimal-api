using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using MinimalAPI.Domain.Interfaces;
using MinimalAPI.Domain.ModelViews;
using MinimalAPI.Domain.Services;
using MinimalAPI.Domain.DTOs;
using MinimalAPI.Infra.DB;
using MinimalAPI.Domain.Entities;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

#region Builder
var builder = WebApplication.CreateBuilder(args);

var jwtKey = builder.Configuration.GetSection("Jwt").ToString();
if (string.IsNullOrEmpty(jwtKey))
{
    throw new Exception("JWT key is not configured.");
}

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        //ValidateIssuer = true,
        ValidateLifetime = true,
        ValidateAudience = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),

    };
});

builder.Services.AddAuthorization();

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
ValidationErrors adminRegisterValidation(AdminDTO adminDTO)
{
    var errors = new ValidationErrors();

    if (string.IsNullOrEmpty(adminDTO.Name))
    {
        errors.Messages.Add("Name is required.");
    }

    if (string.IsNullOrEmpty(adminDTO.Email))
    {
        errors.Messages.Add("Email is required.");
    }
    else if (!adminDTO.Email.Contains("@"))
    {
        errors.Messages.Add("Email must be valid.");
    }

    if (string.IsNullOrEmpty(adminDTO.Password))
    {
        errors.Messages.Add("Password is required.");
    }
    else if (adminDTO.Password.Length < 6)
    {
        errors.Messages.Add("Password must be at least 6 characters long.");
    }

    return errors;
}

string CreateJwtToken(Admin admin, string jwtKey)
{
    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
    var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    var claims = new List<Claim>()
    {
        new Claim("Email", admin.Email),
        new Claim("Role", admin.Role)
    };
    var token = new JwtSecurityToken(
        claims: claims,
        expires: DateTime.Now.AddDays(1),
        signingCredentials: credentials
    );

    return new JwtSecurityTokenHandler().WriteToken(token);
}

app.MapPost("/admin/login", ([FromBody] LoginDTO loginDTO, IAdminService adminService) =>
{
   var admin = adminService.Login(loginDTO);
   if (admin == null)
   {
       return Results.Unauthorized();
   }
   string token = CreateJwtToken(admin, jwtKey);
   var adminMv = new LoggedAdminMv
   {
       Id = admin.Id,
       Name = admin.Name,
       Email = admin.Email,
       Role = admin.Role,
       Token = token
   };
   return Results.Ok(adminMv);
}).WithTags("Admin");

app.MapPost("/admin", ([FromBody] AdminDTO adminRegisterDTO, IAdminService adminService) =>
{
    var errors = adminRegisterValidation(adminRegisterDTO);
    if (errors.Messages.Any())
    {
        return Results.BadRequest(errors);
    }

    var admin = new Admin
    {
        Name = adminRegisterDTO.Name,
        Email = adminRegisterDTO.Email,
        Password = adminRegisterDTO.Password,
        Role = adminRegisterDTO.Role ?? Roles.Admin
    };
    adminService.CreateAdmin(admin);

    var adminMv = new AdminMv
    {
        Id = admin.Id,
        Name = admin.Name,
        Email = admin.Email,
        Role = admin.Role
    };
    return Results.Created($"/admin/{admin.Id}", adminMv);
}).RequireAuthorization().WithTags("Admin");

app.MapGet("/admin", (IAdminService adminService, [FromQuery] int? page = null) =>
{
    var admins = adminService.GetAllAdmins(page ?? 1);
    var adminMvs = admins.Select(a => new AdminMv
    {
        Id = a.Id,
        Name = a.Name,
        Email = a.Email,
        Role = a.Role
    }).ToList();
    return Results.Ok(adminMvs);
}).RequireAuthorization().WithTags("Admin");

app.MapGet("/admin/{id}", ([FromRoute] int id, IAdminService adminService) =>
{
    var admin = adminService.GetAdminById(id);
    if (admin == null)
    {
        return Results.NotFound();
    }
    return Results.Ok(new AdminMv
    {
        Id = admin.Id,
        Name = admin.Name,
        Email = admin.Email,
        Role = admin.Role
    });
}).RequireAuthorization().WithTags("Admin");
#endregion

#region Vehicles
ValidationErrors validateVehicle(VehicleDTO vehicleDTO)
{
    var errors = new ValidationErrors();

    if (string.IsNullOrEmpty(vehicleDTO.Name))
    {
        errors.Messages.Add("Name is required.");
    }

    if (string.IsNullOrEmpty(vehicleDTO.Brand))
    {
        errors.Messages.Add("Brand is required.");
    }

    if (vehicleDTO.Year == 0)
    {
        errors.Messages.Add("Year is required.");
    }

    if (vehicleDTO.Year < 1950 || vehicleDTO.Year > DateTime.Now.Year)
    {
        errors.Messages.Add("Year must be between 1950 and the current year.");
    }

    if (string.IsNullOrEmpty(vehicleDTO.Model))
    {
        errors.Messages.Add("Model is required.");
    }

    return errors;
}

app.MapPost("/vehicles", ([FromBody] VehicleDTO vehicleDTO, IVehicleService vehicleService) =>
{
    var errors = validateVehicle(vehicleDTO);
    if (errors.Messages.Any())
    {
        return Results.BadRequest(errors);
    }

    var vehicle = new Vehicle
    {
        Name = vehicleDTO.Name,
        Brand = vehicleDTO.Brand,
        Year = vehicleDTO.Year,
        Model = vehicleDTO.Model
    };
    vehicleService.CreateVehicle(vehicle);
    return Results.Created($"/vehicles/{vehicle.Id}", vehicle);
}).RequireAuthorization().WithTags("Vehicles");

app.MapGet("/vehicles", (IVehicleService vehicleService, [FromQuery] int? page = null) =>
{
    var vehicles = vehicleService.GetAllVehicles(page ?? 1);
    return Results.Ok(vehicles);
}).RequireAuthorization().WithTags("Vehicles");

app.MapGet("/vehicles/{id}", ([FromRoute] int id, IVehicleService vehicleService) =>
{
    var vehicle = vehicleService.GetVehicleById(id);
    if (vehicle == null)
    {
        return Results.NotFound();
    }
    return Results.Ok(vehicle);
}).RequireAuthorization().WithTags("Vehicles");

app.MapPut("/vehicles/{id}", ([FromRoute] int id, [FromBody] VehicleDTO vehicleDTO, IVehicleService vehicleService) =>
{
    var errors = validateVehicle(vehicleDTO);
    if (errors.Messages.Any())
    {
        return Results.BadRequest(errors);
    }

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
}).RequireAuthorization().WithTags("Vehicles");

app.MapDelete("/vehicles/{id}", ([FromRoute] int id, IVehicleService vehicleService) =>
{
    var vehicle = vehicleService.GetVehicleById(id);
    if (vehicle == null)
    {
        return Results.NotFound();
    }

    vehicleService.DeleteVehicle(vehicle);
    return Results.NoContent();
}).RequireAuthorization().WithTags("Vehicles");
#endregion

#region App
app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

app.Run();
#endregion
