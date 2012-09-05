namespace RestBucks.Tests.Resources
{
  using NUnit.Framework;

  using Nancy;

  using RestBucks.Domain;
  using RestBucks.Resources.Orders.Representations;

  using Util;

  using SharpTestsEx;

  [TestFixture]
  public class WhenUserUpdateAnOrder : ResourceHandlerTestBase
  {
    [Test]
    public void WhenOrderDoesNotExist_ThenReturn404()
    {
      // Arrange 
      var app = CreateAppProxy();

      // Act
      var response = app.Post("/order/123/",
                              with =>
                              {
                                with.HttpRequest();
                                with.Body(new OrderRepresentation().ToXmlString());
                              });
      // Assert
      response.StatusCode.Should().Be.EqualTo(HttpStatusCode.NotFound);
    }

    [Test]
    public void WhenOrderExist_ThenUpdateLocation()
    {
      // Arrange
      var order = new Order {Id = 123, Location = Location.InShop};
      var app = CreateAppProxy(new RepositoryStub<Order>(order));

      // Act
      var response = app.Post("/order/123/",
                              with =>
                              {
                                with.HttpRequest();
                                with.Body(new OrderRepresentation() {Location = Location.TakeAway}.ToXmlString());
                              });

      // Assert
      response.StatusCode.Should().Be.EqualTo(HttpStatusCode.NoContent);
      order.Location.Should().Be.EqualTo(Location.TakeAway);
    }
  }
}