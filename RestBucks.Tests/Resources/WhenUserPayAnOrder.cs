namespace RestBucks.Tests.Resources
{
  using NUnit.Framework;

  using Nancy;

  using RestBucks.Domain;
  using RestBucks.Resources.Orders;
  using RestBucks.Resources.Orders.Representations;
  using Infrastructure.Linking;
  using Util;

  using SharpTestsEx;

  [TestFixture]
  public class WhenUserPayAnOrder : ResourceHandlerTestBase
  {
    private readonly IResourceLinker resourceLinker = new ResourceLinker();

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
                                with.Body(new PaymentRepresentation { CardNumber = "321", CardOwner = "Jose" }.ToXmlString());
                              });
 
      order.Status.Should().Be.EqualTo(OrderStatus.Paid);
      order.Payment.CardOwner.Should().Be.EqualTo("Jose");
      order.Payment.CreditCardNumber.Should().Be.EqualTo("321");
    }
  }
}