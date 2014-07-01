namespace RestBucks.Tests.Resources
{
  using System.Collections.Generic;
  using System.Linq;

  using NUnit.Framework;
  using Orders.Domain;
  using Util;

  using SharpTestsEx;

  using Nancy;
  using Nancy.Testing;

  [TestFixture]
  public class WhenUserGETsACancelledOrder : ResourceHandlerTestBase
  {
    [Test]
    public void WhenOrderIsCancelled_ThenReturn200()
    {
      var orderRepo = new RepositoryStub<Order>(new Order { Id = 123, Status = OrderStatus.Canceled });
      var app = CreateAppProxy(orderRepo);

      var response = app.Get("/trash/order/123/");

      response.StatusCode.Should().Be.EqualTo(HttpStatusCode.OK);
    }

    [Test]
    public void WhenOrderDoesNotExist_ThenReturn404()
    {
      var app = CreateAppProxy();

      var response = app.Get("/trash/order/123");

      response.StatusCode.Should().Be.EqualTo(HttpStatusCode.NotFound);
    }

    [Test]
    public void WhenOrderIsNotCancelled_ThenReturn404()
    {
        var orderRepo = new RepositoryStub<Order>(new Order { Id = 123, Status = OrderStatus.Unpaid });
        var app = CreateAppProxy(orderRepo);

        var response = app.Get("/trash/order/123");

        response.StatusCode.Should().Be.EqualTo(HttpStatusCode.NotFound);
    }

    [Test]
    public void WhenOrderIsCancelled_ThenReturnItsContent()
    {
      // Arrange
      var order = new Order {Id = 123, Location = Location.TakeAway};
      order.AddItem(new OrderItem(latte, 1, 1, new Dictionary<string, string>()));
      order.Status = OrderStatus.Canceled;
      var orderRepo = new RepositoryStub<Order>(order);

      var app = CreateAppProxy(orderRepo);

      //Act
      var response = app.Get("/trash/order/123/",
                             with =>
                             {
                               with.HttpRequest();
                               with.Header("Accept", "application/xml");
                             });

      // Assert
      response.StatusCode.Should().Be.EqualTo(HttpStatusCode.OK);
      var body = response.BodyAsXml();
      body.Descendants()
        .Single(e => e.Name.LocalName == "location")
        .Value.Should().Be.EqualTo("takeAway");
    }

    [Test]
    public void WithRestbuckJsonAcceptHeaderResponseIsJson()
    {
      var orderRepo = new RepositoryStub<Order>(new Order { Id = 123, Location = Location.TakeAway, Status = OrderStatus.Canceled });
      var app = CreateAppProxy(orderRepo);

      var result = app.Get("/trash/order/123/",
                           with =>
                           {
                             with.HttpRequest();
                             with.Header("Accept", "application/vnd.restbucks+json");
                           });

      Assert.That(result.Context.Response.ContentType, Contains.Substring("application/json"));
    }

    [Test]
    public void WithApplicationXmlAcceptHeaderResponseIsXml()
    {
      var orderRepo = new RepositoryStub<Order>(new Order { Id = 123, Location = Location.TakeAway, Status = OrderStatus.Canceled });

      var app = CreateAppProxy(orderRepo);

      // Act
      var result = app.Get("/trash/order/123/",
                           with =>
                           {
                             with.HttpRequest();
                             with.Header("Accept", "application/xml");
                           });

      // Assert
      Assert.That(result.Context.Response.ContentType, Is.EqualTo("application/vnd.restbucks+xml"));
    }
  }
}
