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

}