namespace RestBucks.Tests.Domain
{
  using System.Linq;
  using Nancy.Routing;
  using NUnit.Framework;
  using Orders.Domain;
  using Orders.Representations;
  using Resources;
  using RestBucks.Infrastructure;
  using RestBucks.Infrastructure.Linking;
  using SharpTestsEx;

  public class GivenAReadyOrder
  {
    private Order order;
    private OrderRepresentation representation;

    [SetUp]
    public void SetUp()
    {
      var appHelper = new ResourceHandlerTestBase();
      var app = appHelper.CreateAppProxy();

      order = new Order();
      order.Pay("123", "jose");
      order.Finish();
      representation = OrderRepresentationMapper.Map(order, appHelper.Container.Resolve<ResourceLinker>(), app.Get("/order").Context);
    }

    [Test]
    public void CancelShouldThrow()
    {
      order.Executing(o => o.Cancel("error"))
        .Throws<InvalidOrderOperationException>()
        .And
        .Exception.Message.Should().Be.EqualTo("The order can not be canceled because it is ready.");
    }

    [Test]
    public void PayShouldThrow()
    {
      order.Executing(o => o.Pay("a", "b"))
        .Throws<InvalidOrderOperationException>()
        .And
        .Exception.Message.Should().Be.EqualTo("The order can not be paid because it is ready.");
    }
  }
}