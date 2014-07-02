namespace RestBucks.Tests.Domain
{
  using System.Linq;
  using Nancy.Routing;
  using Nancy.Testing;
  using Nancy.TinyIoc;
  using NUnit.Framework;
  using Orders.Domain;
  using Orders.Representations;
  using Resources;
  using RestBucks.Infrastructure;
  using RestBucks.Infrastructure.Linking;
  using SharpTestsEx;

  [TestFixture]
  public class GivenAPayedOrder
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
      representation = OrderRepresentationMapper.Map(order, appHelper.Container.Resolve<ResourceLinker>(), app.Get("/order").Context);
    }


    [Test]
    public void ThenNextStepsIncludeGet()
    {
      representation.Links
        .Satisfy(
          links =>
          links.Any(l => l.Uri == "http://restbuckson.net/order/0" && l.Relation.EndsWith("docs/order-get.htm")));
    }

    [Test]
    public void CancelShouldThrow()
    {
      order.Executing(o => o.Cancel("error"))
        .Throws<InvalidOrderOperationException>()
        .And
        .Exception.Message.Should().Be.EqualTo("The order can not be canceled because it is paid.");
    }

    [Test]
    public void PayShouldThrow()
    {
      order.Executing(o => o.Pay("123", "jes"))
        .Throws<InvalidOrderOperationException>()
        .And
        .Exception.Message.Should().Be.EqualTo("The order can not be paid because it is paid.");
    }

    [Test]
    public void FinishShouldWork()
    {
      var oldVersion = order.Version;
      order.Finish();

      order.Status.Should().Be.EqualTo(OrderStatus.Ready);
      order.Version.Should().Be.EqualTo(oldVersion + 1);
    }
  }
}