using MinimalAPI.Domain.Entities;
using MinimalAPI.Domain.DTOs;

namespace MinimalAPI.Domain.Interfaces;

public interface IAdminService
{
    Admin? Login(LoginDTO loginDTO);
    Admin CreateAdmin(Admin admin);
    List<Admin> GetAllAdmins(int? pageNumber);
    Admin? GetAdminById(int id);
}