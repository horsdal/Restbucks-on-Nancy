namespace RestBucks.Tests.Resources
{
  using System.Linq;

  using NUnit.Framework;

  using Nancy;
  using Nancy.Testing;

  [TestFixture]
  public class WhenUserGETsMenu : ResourceHandlerTestBase
  {
    [Test]
    public void StatusCodeIs200Ok()
    {
      var app = CreateAppProxy();
      Assert.That(app.Get("/menu/").StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public void ResponseContainsAllProducts([Values("latte", "Other")] string productName)
    {
      var app = CreateAppProxy();
      var res = app.Get("/menu/",
                        with =>
                        {
                          with.HttpRequest();
                          with.Header("Accept", "application/xml");
                        });
      var result = res.BodyAsXml();

      var itemsInResponse = result.Element("menu").Elements("item");

      Assert.That(
        itemsInResponse
          .Single(element => element.Element("Name").Value == productName),
        Is.Not.Null);
    }

    [Test]
    public void ResponseContainsPrice()
    {
      var app = CreateAppProxy();
      var result = app.Get("/menu/",
                        with =>
                        {
                          with.HttpRequest();
                          with.Header("Accept", "application/vnd.restbucks+xml");
                        }).BodyAsXml();

      var itemsInResponse = result.Element("menu").Elements("item");

      Assert.That(
        itemsInResponse
          .Single(element => element.Element("Name").Value == "latte")
          .Element("Price").Value,
        Is.EqualTo("2.5"));
    }

    [Test]
    public void WithApplicationJsonAcceptHeaderResponseIsJson()
    {
      var app = CreateAppProxy();

      var result = app.Get("/menu/",
                           with =>
                           {
                             with.HttpRequest();
                             with.Header("Accept", "application/json");
                           });

      Assert.That(result.Context.Response.ContentType, Is.EqualTo("application/json"));
    }

    [Test]
    public void WithTextJsonAcceptHeaderResponseIsJson()
    {
      var app = CreateAppProxy();

      var result = app.Get("/menu/",
                           with =>
                           {
                             with.HttpRequest();
                             with.Header("Accept", "text/json");
                           });

      Assert.That(result.Context.Response.ContentType, Is.EqualTo("application/json"));
    }

    [Test]
    public void WithRestbuckJsonAcceptHeaderResponseIsJson()
    {
      var app = CreateAppProxy();

      var result = app.Get("/menu/",
                           with =>
                           {
                             with.HttpRequest();
                             with.Header("Accept", "application/vnd.restbucks+json");
                           });

      Assert.That(result.Context.Response.ContentType, Is.EqualTo("application/json"));
    }

    [Test]
    public void WithApplicationXmlAcceptHeaderResponseIsXml()
    {
      var app = CreateAppProxy();

      var result = app.Get("/menu/",
                           with =>
                           {
                             with.HttpRequest();
                             with.Header("Accept", "application/xml");
                           });

      Assert.That(result.Context.Response.ContentType, Is.EqualTo("application/vnd.restbucks+xml"));
    }

    [Test]
    public void WithApplicationYamlAcceptHeaderResponseIsYaml()
    {
      var app = CreateAppProxy();

      var result = app.Get("/menu/",
                           with =>
                           {
                             with.HttpRequest();
                             with.Header("Accept", "application/yaml");
                           });

      Assert.That(result.Context.Response.ContentType, Is.EqualTo("application/vnd.restbucks+yaml"));
    }

    [Test]
    public void WhenMenuHasNotChanged_ThenReturn304()
    {
      // Arrange
      var app = CreateAppProxy();

      // Acr
      var response =
        app.Get("/menu/",
                with =>
                {
                  with.HttpRequest();
                  with.Header("If-None-Match", "\"1\"");
                });

      //Assert
      Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotModified));
    }

    [Test]
    public void WhenMenuHasChanged_ThenReturn200()
    {
      // Arrange
      var app = CreateAppProxy();

      // Act
      var response =
        app.Get("/menu/",
                with =>
                {
                  with.HttpRequest();
                  with.Header("If-None-Match", "\"0\"");
                });

      // Assert
      Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

  }
}
