using System.Net;
using System.Text;
using System.Text.Json;
using MinimalAPI.Domain.DTOs;
using MinimalAPI.Domain.ModelViews;
using Test.Helpers;

namespace Test.Requests;

[TestClass]
public class AdminRequestTest
{
    [ClassInitialize]
    public static void ClassInit(TestContext testContext)
    {
        Setup.ClassInit(testContext);
    }

    [ClassCleanup]
    public static void ClassCleanup()
    {
        Setup.ClassCleanup();
    }

    [TestMethod]
    public async Task TestGetSetProperties()
    {
        Assert.IsNotNull(Setup.client, "Setup.client is null");

        var loginDTO = new LoginDTO
        {
            Email = "admin@admin.com",
            Password = "admin123"
        };

        var content = new StringContent(JsonSerializer.Serialize(loginDTO), Encoding.UTF8, "application/json");

        var response = await Setup.client.PostAsync("/admin/login", content);

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadAsStringAsync();
        var admin = JsonSerializer.Deserialize<LoggedAdminMv>(
            result,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );

        Assert.IsNotNull(admin?.Email ?? "");
        Assert.IsNotNull(admin?.Role ?? "");
        Assert.IsNotNull(admin?.Token ?? "");
    }
}
