using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MinimalAPI.Domain.Entities;
using MinimalAPI.Domain.Services;
using MinimalAPI.Infra.DB;

namespace Test.Domain.Services;

[TestClass]
public class VehicleServiceTest
{
    private DBContext CreateContextTest()
    {
        var assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var path = Path.GetFullPath(Path.Combine(assemblyPath ?? "", "..", ".."));
        var builder = new ConfigurationBuilder()
            .SetBasePath(path ?? Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables();

        var configuration = builder.Build();
        return new DBContext(configuration);
    }

    [TestMethod]
    public void VehicleCreationTest()
    {
        var vehicle = new Vehicle();
        vehicle.Brand = "Top Brand";
        vehicle.Model = "Top Model";
        vehicle.Name = "Top Top";
        vehicle.Year = 2002;

        var context = CreateContextTest();
        context.Database.ExecuteSqlRaw("TRUNCATE TABLE Vehicles");
        var vehicleService = new VehicleService(context);

        vehicleService.CreateVehicle(vehicle);
        var vehicleResult = vehicleService.GetVehicleById(vehicle.Id);

        Assert.AreEqual(1, vehicleResult.Id);
    }
}