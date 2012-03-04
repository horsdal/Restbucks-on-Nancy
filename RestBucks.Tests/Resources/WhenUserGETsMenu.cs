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
      var result = app.Get("/menu/").BodyAsXml();

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
      var result = app.Get("/menu/").BodyAsXml();

      var itemsInResponse = result.Element("menu").Elements("item");

      Assert.That(
        itemsInResponse
        .Single(element => element.Element("Name").Value == "latte")
        .Element("Price").Value,
        Is.EqualTo("2.5")); 
    }
  }
}
