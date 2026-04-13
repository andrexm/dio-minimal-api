using Microsoft.EntityFrameworkCore;
using MinimalAPI.Domain.Entities;

namespace MinimalAPI.Infra.DB;

public class DBContext : DbContext
{
    private readonly IConfiguration _configuration;

    public DbSet<Admin> Admins { get; set; } = default!;

    public DBContext(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var connectionString = _configuration.GetConnectionString("mysql")?.ToString();
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Connection string 'mysql' not found.");
            }

            optionsBuilder.UseMySql(
                connectionString,
                ServerVersion.AutoDetect(connectionString)
            );
        }
    }
}