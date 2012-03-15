namespace RestBucks.Tests.Resources
{
  using NUnit.Framework;

  using Nancy;

  using RestBucks.Domain;
  using RestBucks.Resources.Orders.Representations;

  using Util;

  using SharpTestsEx;

  [TestFixture]
  public class WhenUserCancelAnOrder : ResourceHandlerTestBase
  {
    [Test]
    public void WhenOrderDoesNotExist_ThenReturn404()
    {
      // Arrange
      var app = CreateAppProxy();

      // Act
      var response = app.Delete("/order/123/");

      // Assert
      response.StatusCode.Should().Be.EqualTo(HttpStatusCode.NotFound);
    }

    [Test]
    public void WhenOrderExist_ThenCancel()
    {
      // Arrange
      var order = new Order { Id = 123 };
      var app = CreateAppProxy(new RepositoryStub<Order>(order));

      // Act
      app.Delete("/order/123/");

      // Assert
      order.Status.Should().Be.EqualTo(OrderStatus.Canceled);
    }

    [Test]
    public void WhenOrderExist_ThenCancelAndReturn204()
    {
      // Arrange
      var order = new Order { Id = 123 };
      var app = CreateAppProxy(new RepositoryStub<Order>(order));

      // Act
      var response = app.Delete("/order/123/");

      // Assert
      response.StatusCode.Should().Be.EqualTo(HttpStatusCode.NoContent);
    }

    [Test]
    public void ACallToGet_ShouldReturnMovedPermanentlyAndNewLocation()
    {
      // Arrange
      var order = new Order { Id = 123 };
      var app = CreateAppProxy(new RepositoryStub<Order>(order));

      // Act
      var response = app.Delete("/order/123/");
      var responseToGet = app.Get("/order/123/");

      var expected = "http://restbuckson.net/trash/order/123";
      responseToGet.StatusCode.Should().Be.EqualTo(HttpStatusCode.MovedPermanently);
      responseToGet.Headers.ContainsKey("Location").Should().Be.True();
      responseToGet.Headers["Location"].Should().Be.EqualTo(expected);
    }

    [Test, Ignore]
    public void ACallToGetCanceled_ShouldReturnTheOrder()
    {
      // Arrange
      var order = new Order { Id = 123 };
      var expectedBody = OrderRepresentationMapper.Map(order).ToXmlString();
      var app = CreateAppProxy(new RepositoryStub<Order>(order));

      // Act
      var response = app.Delete("/order/123/");
      var responseToGet = app.Get("/trash/order/123/");


      responseToGet.StatusCode.Should().Be.EqualTo(HttpStatusCode.OK);
      responseToGet.Body.ToXmlString().Should().Be.EqualTo(expectedBody);
    }
  }
}