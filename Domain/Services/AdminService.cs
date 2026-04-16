using MinimalAPI.Domain.Entities;
using MinimalAPI.Domain.Interfaces;
using MinimalAPI.Domain.DTOs;
using MinimalAPI.Infra.DB;

namespace MinimalAPI.Domain.Services;

public class AdminService : IAdminService
{
    private readonly DBContext _db;

    public AdminService(DBContext db)
    {
        _db = db;
    }

    public Admin? Login(LoginDTO loginDTO)
    {
        var admin = _db.Admins.FirstOrDefault(a => a.Email == loginDTO.Email && a.Password == loginDTO.Password);
        return admin ?? null;
    }

    public Admin CreateAdmin(Admin admin)
    {
        _db.Admins.Add(admin);
        _db.SaveChanges();
        return admin;
    }

    public Admin? GetAdminById(int id)
    {
        var admin = _db.Admins.FirstOrDefault(a => a.Id == id);
        return admin ?? null;
    }

    public List<Admin> GetAllAdmins(int? pageNumber = null)
    {
        var query = _db.Admins.AsQueryable();
        if (pageNumber.HasValue)
        {
            query = query.Skip((pageNumber.Value - 1) * 10).Take(10);
        }
        return query.ToList();
    }
}