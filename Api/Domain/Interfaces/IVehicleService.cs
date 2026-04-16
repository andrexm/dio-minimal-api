using MinimalAPI.Domain.Entities;

namespace MinimalAPI.Domain.Interfaces;

public interface IVehicleService
{
    List<Vehicle> GetAllVehicles(int pageNumber, string? name = null, string? brand = null);
    Vehicle? GetVehicleById(int id);
    void CreateVehicle(Vehicle vehicle);
    void UpdateVehicle(Vehicle vehicle);
    void DeleteVehicle(Vehicle vehicle);
}
