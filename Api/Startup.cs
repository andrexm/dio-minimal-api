using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MinimalAPI.Domain.DTOs;
using MinimalAPI.Domain.Entities;
using MinimalAPI.Domain.Interfaces;
using MinimalAPI.Domain.ModelViews;
using MinimalAPI.Domain.Services;
using MinimalAPI.Infra.DB;

namespace MinimalAPI;

public class Startup
{
    public IConfiguration Configuration {get; set;} = default!;
    private string JwtKey = "";

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
        JwtKey = Configuration.GetSection("Jwt").ToString() ?? "1234";
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateLifetime = true,
                ValidateAudience = false,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtKey)),
            };
        });

        services.AddAuthorization();

        services.AddScoped<IAdminService, AdminService>();
        services.AddScoped<IVehicleService, VehicleService>();

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Provide a JWT Token here."
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        },
                    },
                    new string[]{}
                }
            });
        });

        services.AddDbContext<DBContext>(options =>
        {
            options.UseMySql(
                Configuration.GetConnectionString("mysql"),
                ServerVersion.AutoDetect(Configuration.GetConnectionString("mysql"))
            );
        });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            #region Home
            endpoints.MapGet("/", () =>
            {
                return Results.Json(new Home());
            }).AllowAnonymous().WithTags("Home");

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

            string CreateJwtToken(Admin admin, string JwtKey)
            {
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtKey));
                var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var claims = new List<Claim>()
                {
                    new Claim("Email", admin.Email),
                    new Claim(ClaimTypes.Role, admin.Role)
                };
                var token = new JwtSecurityToken(
                    claims: claims,
                    expires: DateTime.Now.AddDays(1),
                    signingCredentials: credentials
                );

                return new JwtSecurityTokenHandler().WriteToken(token);
            }

            endpoints.MapPost("/admin/login", ([FromBody] LoginDTO loginDTO, IAdminService adminService) =>
            {
            var admin = adminService.Login(loginDTO);
            if (admin == null)
            {
                return Results.Unauthorized();
            }
            string token = CreateJwtToken(admin, JwtKey);
            var adminMv = new LoggedAdminMv
            {
                Id = admin.Id,
                Name = admin.Name,
                Email = admin.Email,
                Role = admin.Role,
                Token = token
            };
            return Results.Ok(adminMv);
            }).AllowAnonymous().WithTags("Admin");

            endpoints.MapPost("/admin", ([FromBody] AdminDTO adminRegisterDTO, IAdminService adminService) =>
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
            })
                .RequireAuthorization()
                .RequireAuthorization(new AuthorizeAttribute {Roles = Roles.Admin})
                .WithTags("Admin");

            endpoints.MapGet("/admin", (IAdminService adminService, [FromQuery] int? page = null) =>
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
            })
                .RequireAuthorization()
                .RequireAuthorization(new AuthorizeAttribute {Roles = Roles.Admin})
                .WithTags("Admin");

            endpoints.MapGet("/admin/{id}", ([FromRoute] int id, IAdminService adminService) =>
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
            })
                .RequireAuthorization()
                .RequireAuthorization(new AuthorizeAttribute {Roles = Roles.Admin})
                .WithTags("Admin");
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

            endpoints.MapPost("/vehicles", ([FromBody] VehicleDTO vehicleDTO, IVehicleService vehicleService) =>
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
            })
                .RequireAuthorization()
                .RequireAuthorization(new AuthorizeAttribute {Roles = $"{Roles.Admin},{Roles.Editor}"})
                .WithTags("Vehicles");

            endpoints.MapGet("/vehicles", (IVehicleService vehicleService, [FromQuery] int? page = null) =>
            {
                var vehicles = vehicleService.GetAllVehicles(page ?? 1);
                return Results.Ok(vehicles);
            })
                .RequireAuthorization()
                .RequireAuthorization(new AuthorizeAttribute {Roles = $"{Roles.Admin},{Roles.Editor}"})
                .WithTags("Vehicles");

            endpoints.MapGet("/vehicles/{id}", ([FromRoute] int id, IVehicleService vehicleService) =>
            {
                var vehicle = vehicleService.GetVehicleById(id);
                if (vehicle == null)
                {
                    return Results.NotFound();
                }
                return Results.Ok(vehicle);
            })
                .RequireAuthorization()
                .RequireAuthorization(new AuthorizeAttribute {Roles = $"{Roles.Admin},{Roles.Editor}"})
                .WithTags("Vehicles");

            endpoints.MapPut("/vehicles/{id}", ([FromRoute] int id, [FromBody] VehicleDTO vehicleDTO, IVehicleService vehicleService) =>
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
            })
                .RequireAuthorization()
                .RequireAuthorization(new AuthorizeAttribute {Roles = Roles.Admin})
                .WithTags("Vehicles");

            endpoints.MapDelete("/vehicles/{id}", ([FromRoute] int id, IVehicleService vehicleService) =>
            {
                var vehicle = vehicleService.GetVehicleById(id);
                if (vehicle == null)
                {
                    return Results.NotFound();
                }

                vehicleService.DeleteVehicle(vehicle);
                return Results.NoContent();
            })
                .RequireAuthorization()
                .RequireAuthorization(new AuthorizeAttribute {Roles = Roles.Admin})
                .WithTags("Vehicles");
            #endregion
        });
    }
}