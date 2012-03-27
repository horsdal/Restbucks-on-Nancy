namespace RestBucks.Tests.Domain
{
  using NUnit.Framework;

  using RestBucks.Domain;

  using SharpTestsEx;

  [TestFixture]
  public class OrderTests
  {
    [Test]
    public void IfTheOrderDoesNotHaveItems_ThenReturnInvalidMessage()
    {
      var order = new Order();
      order.GetErrorMessages()
        .Should().Contain("The order must include at least one item.");
    }

    [Test]
    public void IfItemHasQuantity0_ThenReturnInvalidMessage()
    {
      var order = new Order();
      order.AddItem(new OrderItem {Quantity = 0});
      order.GetErrorMessages()
        .Should().Contain("Item 0: Quantity should be greater than 0.");
    }

    [Test]
    public void IfTheProductDoesNotAllowCustomization_ThenReturnInvalidMessage()
    {
      var product = new Product
                    {
                      Name = "latte",
                      Customizations = {new Customization {Name = "size", PossibleValues = {"medium", "large"}}}
                    };

      var order = new Order();
      order.AddItem(new OrderItem
                    {
                      Quantity = 1,
                      Product = product,
                      Preferences = {{"milk", "lot"}}
                    });
      order.GetErrorMessages()
        .Should().Contain("Item 0: The product latte does not have a customization: milk/lot.");
    }
  }
}