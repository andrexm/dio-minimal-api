using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MinimalAPI.Domain.Interfaces;
using MinimalAPI.Domain.Services;
using MinimalAPI.DTOs;
using MinimalAPI.Infra.DB;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IAdminService, AdminService>();

builder.Services.AddDbContext<DBContext>(options =>
{
    options.UseMySql(
        builder.Configuration.GetConnectionString("mysql"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("mysql"))
    );
});


var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapPost("/login", ([FromBody] LoginDTO loginDTO, IAdminService adminService) =>
{
   var admin = adminService.Login(loginDTO);
   if (admin == null)
   {
       return Results.Unauthorized();
   }
   return Results.Ok(admin);
});

app.Run();
