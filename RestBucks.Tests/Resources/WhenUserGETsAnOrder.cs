namespace RestBucks.Tests.Resources
{
  using System.Collections.Generic;
  using System.Linq;
  using System.Xml.Linq;

  using NUnit.Framework;

  using RestBucks.Domain;

  using Util;

  using SharpTestsEx;

  using Nancy;
  using Nancy.Testing;

  [TestFixture]
  public class WhenUserGETsAnOrder : ResourceHandlerTestBase
  {
    [Test]
    public void WhenOrderHasNotChanged_ThenReturn304()
    {
      // Arrange
      var orderRepo = new RepositoryStub<Order>(new Order {Version = 1, Id = 123});
      var app = CreateAppProxy(orderRepo);

      // Acr
      var response =
        app.Get("/order/123/",
                with =>
                {
                  with.HttpRequest();
                  with.Header("If-None-Match", "\"1\"");
                });

      //Assert
      response.StatusCode.Should().Be.EqualTo(HttpStatusCode.NotModified);
    }

    [Test]
    public void WhenOrderHasChanged_ThenReturn200()
    {
      // Arrange
      var orderRepo = new RepositoryStub<Order>(new Order {Version = 2, Id = 123});
      var app = CreateAppProxy(orderRepo);

      // Act
      var response =
        app.Get("/order/123/",
                with =>
                {
                  with.HttpRequest();
                  with.Header("If-None-Match", "\"1\"");
                });

      // Assert
      response.StatusCode.Should().Be.EqualTo(HttpStatusCode.OK);
    }

    [Test]
    public void WhenOrderDoesNotExist_ThenReturn404()
    {
      //Arrange 
      var app = CreateAppProxy();

      // Act
      var response = app.Get("/order/123");

      //Assert
      response.StatusCode.Should().Be.EqualTo(HttpStatusCode.NotFound);
    }

    [Test]
    public void WhenOrderExists_ThenReturnItsContent()
    {
      // Arrange
      var order = new Order {Id = 123, Location = Location.TakeAway};
      order.AddItem(new OrderItem(latte, 1, 1, new Dictionary<string, string>()));
      var orderRepo = new RepositoryStub<Order>(order);

      var app = CreateAppProxy(orderRepo);

      //Act
      var response = app.Get("/order/123/");

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
      // Arrange
      var order = new Order { Id = 123, Location = Location.TakeAway };
      order.AddItem(new OrderItem(latte, 1, 1, new Dictionary<string, string>()));
      var orderRepo = new RepositoryStub<Order>(order);

      var app = CreateAppProxy(orderRepo);

      // Act
      var result = app.Get("/order/123/",
                           with =>
                           {
                             with.HttpRequest();
                             with.Header("Accept", "application/vnd.restbucks+json");
                           });

      // Assert
      Assert.That(result.Context.Response.ContentType, Is.EqualTo("application/json"));
    }

    [Test]
    public void WithApplicationXmlAcceptHeaderResponseIsXml()
    {
      // Arrange
      var order = new Order { Id = 123, Location = Location.TakeAway };
      order.AddItem(new OrderItem(latte, 1, 1, new Dictionary<string, string>()));
      var orderRepo = new RepositoryStub<Order>(order);

      var app = CreateAppProxy(orderRepo);

      // Act
      var result = app.Get("/order/123/",
                           with =>
                           {
                             with.HttpRequest();
                             with.Header("Accept", "application/xml");
                           });

      // Assert
      Assert.That(result.Context.Response.ContentType, Is.EqualTo("application/xml"));
    }


  }
}
