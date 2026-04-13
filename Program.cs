using Microsoft.EntityFrameworkCore;
using MinimalAPI.DTOs;
using MinimalAPI.Infra.DB;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<DBContext>(options =>
{
    options.UseMySql(
        builder.Configuration.GetConnectionString("mysql"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("mysql"))
    );
});


var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapPost("/login", (LoginDTO loginDTO) =>
{
   if (loginDTO.Email == "email@test.com" && loginDTO.Password == "password")
   {
       return Results.Ok("Login successful");
   }
   else
   {
       return Results.Unauthorized();
   }
});

app.Run();
