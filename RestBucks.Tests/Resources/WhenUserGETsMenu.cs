namespace RestBucks.Tests.Resources
{
  using System.Linq;
  using System.Xml.Linq;

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
    public void ResponseContainsAllProducts()
    {
      var app = CreateAppProxy();
      var result = app.Get("/menu/").BodyAsXml();

      var itemsInResponse = result.Element("menu").Elements("item");

      Assert.That(
        itemsInResponse
        .Where(element => element.Element("Name").Value == "latte")
        .ToList(), Is.Not.Empty);
      
      Assert.That(
        itemsInResponse
        .Where(element => element.Element("Name").Value == "Other")
        .ToList(), Is.Not.Empty);
    }
  }
}
