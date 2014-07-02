namespace RestBucks.Tests.Domain
{
  using Nancy.Routing;
  using NUnit.Framework;
  using Orders.Domain;
  using Orders.Representations;
  using Resources;
  using RestBucks.Infrastructure;
  using RestBucks.Infrastructure.Linking;
  using SharpTestsEx;

  [TestFixture]
  public class GivenACanceledOrder
  {
    private Order order;
    private OrderRepresentation representation;

    [SetUp]
    public void SetUp()
    {
      var appHelper = new ResourceHandlerTestBase();
      var app = appHelper.CreateAppProxy();

      order = new Order();
      order.Cancel("You are too slow.");
      representation = OrderRepresentationMapper.Map(order, appHelper.Container.Resolve<ResourceLinker>(), app.Get("/order").Context);
    }

    [Test]
    public void NextStepsShouldBeEmpty()
    {
      representation.Links.Should().Be.Empty();
    }

    [Test]
    public void CancelShouldThrow()
    {
      order.Executing(o => o.Cancel("error"))
        .Throws<InvalidOrderOperationException>()
        .And
        .Exception.Message.Should().Be.EqualTo("The order can not be canceled because it is canceled.");
    }

    [Test]
    public void AddItemShouldThrow()
    {
      order.Executing(o => o.AddItem(new OrderItem()))
        .Throws<InvalidOrderOperationException>()
        .And
        .Exception.Message.Should().Be.EqualTo("Can't add another item to the order because it is canceled.");
    }

    [Test]
    public void PayShouldThrow()
    {
      order.Executing(o => o.Pay("a", "b"))
        .Throws<InvalidOrderOperationException>()
        .And
        .Exception.Message.Should().Be.EqualTo("The order can not be paid because it is canceled.");
    }
  }
}