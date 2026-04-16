using MinimalAPI.Domain.Entities;
using MinimalAPI.Domain.Interfaces;
using MinimalAPI.Infra.DB;

namespace MinimalAPI.Domain.Services;

public class VehicleService : IVehicleService
{
    private readonly DBContext _db;

    public VehicleService(DBContext db)
    {
        _db = db;
    }

    public List<Vehicle> GetAllVehicles(int pageNumber, string? name = null, string? brand = null)
    {
        var query = _db.Vehicles.AsQueryable();

        if (!string.IsNullOrEmpty(name))
        {
            query = query.Where(v => v.Name.Contains(name));
        }

        if (!string.IsNullOrEmpty(brand))
        {
            query = query.Where(v => v.Brand.Contains(brand));
        }

        return query.Skip((pageNumber - 1) * 10).Take(10).ToList();
    }

    public Vehicle? GetVehicleById(int id)
    {
        return _db.Vehicles.FirstOrDefault(v => v.Id == id) ?? null;
    }

    public void CreateVehicle(Vehicle vehicle)
    {
        _db.Vehicles.Add(vehicle);
        _db.SaveChanges();
    }

    public void UpdateVehicle(Vehicle vehicle)
    {
        _db.Vehicles.Update(vehicle);
        _db.SaveChanges();
    }

    public void DeleteVehicle(Vehicle vehicle)
    {
        _db.Vehicles.Remove(vehicle);
        _db.SaveChanges();
    }
}
