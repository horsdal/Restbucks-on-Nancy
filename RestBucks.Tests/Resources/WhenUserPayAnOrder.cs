namespace RestBucks.Tests.Resources
{
  using NUnit.Framework;

  using Nancy;

  using RestBucks.Domain;
  using RestBucks.Resources.Orders.Representations;

  using Util;

  using SharpTestsEx;

  [TestFixture]
  public class WhenUserPayAnOrder : ResourceHandlerTestBase
  {
    [Test]
    public void WhenOrderDoesNotExist_ThenReturn404()
    {
      // Arrange
      var app = CreateAppProxy();

      // Act
      var response = app.Post("/order/123/payment",
                              with =>
                              {
                                with.HttpRequest();
                                with.Body(new PaymentRepresentation().ToXmlString());
                              });

      // Assert
      response.StatusCode.Should().Be.EqualTo(HttpStatusCode.NotFound);
    }

    [Test]
    public void WhenOrderExist_ThenCancel()
    {
      //Arrange
      var order = new Order {Id = 123};
      var app = CreateAppProxy(new RepositoryStub<Order>(order));

      // Act
      var response = app.Post("/order/123/payment/",
                              with =>
                              {
                                with.HttpRequest();
                                var xmlString = new PaymentRepresentation {CardNumber = "321", CardOwner = "Jose"}.ToXmlString();
                                with.Body(xmlString);
                              });
 
      order.Status.Should().Be.EqualTo(OrderStatus.Paid);
      order.Payment.CardOwner.Should().Be.EqualTo("Jose");
      order.Payment.CreditCardNumber.Should().Be.EqualTo("321");
    }
  }
}