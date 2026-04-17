using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MinimalAPI.Domain.Entities;
using MinimalAPI.Domain.Services;
using MinimalAPI.Infra.DB;

namespace Test.Domain.Services;

[TestClass]
public class AdminServiceTest
{
    private DBContext CreateContextTest()
    {
        var assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var path = Path.GetFullPath(Path.Combine(assemblyPath ?? "", "..", ".."));
        var builder = new ConfigurationBuilder()
            .SetBasePath(path ?? Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables();

        var configuration = builder.Build();
        return new DBContext(configuration);
    }

    [TestMethod]
    public void AdminCreationTest()
    {
        var admin = new Admin();
        admin.Email = "test@test.com";
        admin.Password = "strongstring";
        admin.Role = "Admin";

        var context = CreateContextTest();
        context.Database.ExecuteSqlRaw("TRUNCATE TABLE Admins");
        var adminService = new AdminService(context);

        adminService.CreateAdmin(admin);
        var adminResult = adminService.GetAdminById(admin.Id);

        Assert.AreEqual(1, adminService.GetAllAdmins(1).Count());
        Assert.AreEqual(1, adminResult.Id);
    }
}