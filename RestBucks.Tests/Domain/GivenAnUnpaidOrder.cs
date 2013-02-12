namespace RestBucks.Tests.Domain
{
  using System.Linq;

  using NUnit.Framework;
  using Orders.Domain;
  using Orders.Representations;
  using SharpTestsEx;

  [TestFixture]
  public class GivenAnUnpaidOrder
  {
    private Order order;
    private OrderRepresentation representation;

    [SetUp]
    public void SetUp()
    {
      order = new Order();
      representation = OrderRepresentationMapper.Map(order, "http://restbuckson.net/");
    }

    [Test]
    public void ThenNextStepsIncludeCancel()
    {
      representation.Links
        .Satisfy(
          links =>
          links.Any(l => l.Uri == "http://restbuckson.net/order/0" && l.Relation.EndsWith("docs/order-cancel.htm")));
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
    public void TheNextStepsIncludeUpdate()
    {
      representation.Links
        .Satisfy(
          links =>
          links.Any(l => l.Uri == "http://restbuckson.net/order/0" && l.Relation.EndsWith("docs/order-update.htm")));
    }

    [Test]
    public void TheNextStepsIncludePay()
    {
      representation.Links
        .Satisfy(
          links =>
          links.Any(l => l.Uri == "http://restbuckson.net/order/0/payment" && l.Relation.EndsWith("docs/order-pay.htm")));
    }

    [Test]
    public void NextStepShouldNotIncludeReceipt()
    {
      representation.Links
        .Satisfy(
          links =>
          !links.Any(
            l => l.Uri == "http://restbuckson.net/order/ready/0" && l.Relation.EndsWith("docs/receipt-coffee.htm")));
    }

    [Test]
    public void CancelShouldWork()
    {
      var oldVersion = order.Version;
      order.Cancel("error");
      order.Status.Should().Be.EqualTo(OrderStatus.Canceled);
      order.Version.Should().Be.EqualTo(oldVersion + 1);
    }

    [Test]
    public void PayShouldWork()
    {
      var oldVersion = order.Version;
      order.Pay("Jose", "123123123");
      order.Status.Should().Be.EqualTo(OrderStatus.Paid);
      order.Version.Should().Be.EqualTo(oldVersion + 1);
    }
  }
}