using MinimalAPI.Domain.DTOs;
using MinimalAPI.Domain.Entities;
using MinimalAPI.Domain.Interfaces;

namespace Test.Mocks;

public class AdminServiceMock : IAdminService
{
    private static List<Admin> admins = new List<Admin>();

    static AdminServiceMock()
    {
        // Initialize with test data
        admins.Add(new Admin
        {
            Id = 1,
            Name = "Admin",
            Email = "admin@admin.com",
            Password = "admin123",
            Role = "Admin"
        });
    }

    public Admin? Login(LoginDTO loginDTO)
    {
        return admins.FirstOrDefault(a => a.Email == loginDTO.Email && a.Password == loginDTO.Password) ?? null!;
    }

    public Admin GetAdminById(int id)
    {
        return admins.FirstOrDefault(a => a.Id == id) ?? null!;
    }

    public Admin CreateAdmin(Admin admin)
    {
        admin.Id = admins.Count + 1;
        admins.Add(admin);
        return admin;
    }

    public List<Admin> GetAllAdmins(int? pageNumber)
    {
        return admins;
    }
}