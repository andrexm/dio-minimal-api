using MinimalAPI.DTOs;

var builder = WebApplication.CreateBuilder(args);
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
