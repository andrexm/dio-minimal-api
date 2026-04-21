using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using MinimalAPI.Domain.DTOs;
using MinimalAPI.Domain.Entities;
using MinimalAPI.Domain.ModelViews;
using Test.Helpers;

namespace Test.Requests;

[TestClass]
public class VehicleRequestTest
{
    private string? token;

    [ClassInitialize]
    public static void ClassInit(TestContext testContext)
    {
        Setup.ClassInit(testContext);
    }

    [ClassCleanup]
    public static void ClassCleanup()
    {
        Setup.ClassCleanup();
    }

    [TestInitialize]
    public async Task TestInitialize()
    {
        // Login to get token
        var loginDTO = new LoginDTO
        {
            Email = "admin@admin.com",
            Password = "admin123"
        };

        var content = new StringContent(JsonSerializer.Serialize(loginDTO), Encoding.UTF8, "application/json");
        var response = await Setup.client.PostAsync("/admin/login", content);

        var result = await response.Content.ReadAsStringAsync();
        var admin = JsonSerializer.Deserialize<LoggedAdminMv>(
            result,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );

        token = admin?.Token;
        Setup.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    [TestMethod]
    public async Task TestCreateVehicle()
    {
        // Arrange
        var vehicleDTO = new VehicleDTO
        {
            Name = "Civic",
            Brand = "Honda",
            Year = 2020,
            Model = "EX"
        };

        var content = new StringContent(JsonSerializer.Serialize(vehicleDTO), Encoding.UTF8, "application/json");

        // Act
        var response = await Setup.client.PostAsync("/vehicles", content);

        // Assert
        Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);

        var result = await response.Content.ReadAsStringAsync();
        var vehicle = JsonSerializer.Deserialize<Vehicle>(
            result,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );

        Assert.IsNotNull(vehicle);
        Assert.AreEqual(vehicleDTO.Name, vehicle.Name);
        Assert.AreEqual(vehicleDTO.Brand, vehicle.Brand);
        Assert.AreEqual(vehicleDTO.Year, vehicle.Year);
        Assert.AreEqual(vehicleDTO.Model, vehicle.Model);
    }

    [TestMethod]
    public async Task TestGetAllVehicles()
    {
        // Act
        var response = await Setup.client.GetAsync("/vehicles?page=1");

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadAsStringAsync();
        var vehicles = JsonSerializer.Deserialize<List<Vehicle>>(
            result,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );

        Assert.IsNotNull(vehicles);
    }

    [TestMethod]
    public async Task TestGetVehicleById()
    {
        // First create a vehicle
        var vehicleDTO = new VehicleDTO
        {
            Name = "Corolla",
            Brand = "Toyota",
            Year = 2021,
            Model = "LE"
        };

        var content = new StringContent(JsonSerializer.Serialize(vehicleDTO), Encoding.UTF8, "application/json");
        var createResponse = await Setup.client.PostAsync("/vehicles", content);
        var createResult = await createResponse.Content.ReadAsStringAsync();
        var createdVehicle = JsonSerializer.Deserialize<Vehicle>(
            createResult,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );

        // Act
        var response = await Setup.client.GetAsync($"/vehicles/{createdVehicle?.Id}");

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadAsStringAsync();
        var vehicle = JsonSerializer.Deserialize<Vehicle>(
            result,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );

        Assert.IsNotNull(vehicle);
        Assert.AreEqual(createdVehicle?.Id, vehicle.Id);
        Assert.AreEqual(vehicleDTO.Name, vehicle.Name);
    }
}
