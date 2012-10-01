using RestBucks.Infrastructure;

namespace RestBucks.Tests.Domain
{
  using System.Linq;

  using NUnit.Framework;

  using RestBucks.Domain;
  using Infrastructure;
  using RestBucks.Resources.Orders.Representations;

  using SharpTestsEx;

  [TestFixture]
  public class GivenAPayedOrder
  {
    private Order order;
    private OrderRepresentation representation;

    [SetUp]
    public void SetUp()
    {
      order = new Order();
      order.Pay("123", "jose");
      representation = OrderRepresentationMapper.Map(order, "http://restbuckson.net/");
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