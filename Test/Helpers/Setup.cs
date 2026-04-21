using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using MinimalAPI;
using MinimalAPI.Domain.Interfaces;
using Test.Mocks;

namespace Test.Helpers;

public class Setup
{
    public const string PORT = "5000";
    public static TestContext testContext { get; set; } = default!;
    public static WebApplicationFactory<Startup> http { get; set; } = default!;
    public static HttpClient client { get; set; } = default!;

    public static void ClassInit(TestContext testContext)
    {
        Setup.testContext = testContext;
        Setup.http = new WebApplicationFactory<Startup>();

        Setup.http = Setup.http.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.AddScoped<IAdminService, AdminServiceMock>();
                services.AddScoped<IVehicleService, VehicleServiceMock>();
            });
        });

        Setup.client = Setup.http.CreateClient();
    }

    public static void ClassCleanup()
    {
        Setup.http.Dispose();
    }
}