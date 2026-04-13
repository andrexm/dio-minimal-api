using MinimalAPI.Domain.Entities;
using MinimalAPI.DTOs;

namespace MinimalAPI.Domain.Interfaces;

public interface IAdminService
{
    Admin? Login(LoginDTO loginDTO);   
}