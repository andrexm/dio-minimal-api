using MinimalAPI.Domain.Entities;

namespace Test.Domain.Entities;

[TestClass]
public class VehicleTest
{
    [TestMethod]
    public void TestGetSetProperties()
    {
        var vehicle = new Vehicle();

        vehicle.Id = 1;
        vehicle.Brand = "Brand";
        vehicle.Model = "Model";
        vehicle.Year = 2013;
        vehicle.Name = "Mega";

        Assert.AreEqual(1, vehicle.Id);
        Assert.AreEqual("Brand", vehicle.Brand);
        Assert.AreEqual("Model", vehicle.Model);
        Assert.AreEqual(2013, vehicle.Year);
        Assert.AreEqual("Mega", vehicle.Name);
    }
}