using MinimalAPI.Domain.Entities;
using MinimalAPI.Domain.Interfaces;

namespace Test.Mocks;

public class VehicleServiceMock : IVehicleService
{
    private static List<Vehicle> vehicles = new List<Vehicle>();

    public List<Vehicle> GetAllVehicles(int pageNumber, string? name = null, string? brand = null)
    {
        var query = vehicles.AsQueryable();

        if (!string.IsNullOrEmpty(name))
        {
            query = query.Where(v => v.Name.Contains(name, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrEmpty(brand))
        {
            query = query.Where(v => v.Brand.Contains(brand, StringComparison.OrdinalIgnoreCase));
        }

        return query.Skip((pageNumber - 1) * 10).Take(10).ToList();
    }

    public Vehicle? GetVehicleById(int id)
    {
        return vehicles.FirstOrDefault(v => v.Id == id);
    }

    public void CreateVehicle(Vehicle vehicle)
    {
        vehicle.Id = vehicles.Count + 1;
        vehicles.Add(vehicle);
    }

    public void UpdateVehicle(Vehicle vehicle)
    {
        var existingVehicle = vehicles.FirstOrDefault(v => v.Id == vehicle.Id);
        if (existingVehicle != null)
        {
            existingVehicle.Name = vehicle.Name;
            existingVehicle.Brand = vehicle.Brand;
            existingVehicle.Model = vehicle.Model;
            existingVehicle.Year = vehicle.Year;
        }
    }

    public void DeleteVehicle(Vehicle vehicle)
    {
        vehicles.Remove(vehicle);
    }
}
