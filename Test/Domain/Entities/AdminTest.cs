using MinimalAPI.Domain.Entities;

namespace Test.Domain.Entities;

[TestClass]
public class AdminTest
{
    [TestMethod]
    public void TestGetSetProperties()
    {
        var admin = new Admin();

        admin.Id = 1;
        admin.Email = "test@test.com";
        admin.Password = "strongstring";
        admin.Role = "Admin";

        Assert.AreEqual(1, admin.Id);
        Assert.AreEqual("test@test.com", admin.Email);
        Assert.AreEqual("strongstring", admin.Password);
        Assert.AreEqual("Admin", admin.Role);
    }
}