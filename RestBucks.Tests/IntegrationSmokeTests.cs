namespace RestBucks.Tests
{
  using NUnit.Framework;

  using Nancy;
  using Nancy.Testing;

  using RestBucks;

  [TestFixture]
  public class IntegrationSmokeTests
  {
    [Test]
    public void AppCanInitializeWithRealDependencies()
    {
      Assert.DoesNotThrow(
        () => new Browser(new Bootstrapper())
        );
    }

    [Test]
    public void AppCanCreateAndDeleteOrder()
    {
      var app = new Browser(new Bootstrapper());

      var createdResponse = app.Post("/orders/",
                                     with =>
                                     {
                                       with.HttpRequest();
                                       with.Header("Content-Type", "application/vnd.restbucks+xml");
                                       with.Body("<order xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns=\"http://restbuckson.net\">"
                                                 + "    <location>inShop</location>"
                                                 + "    <items>"
                                                 + "    <item>"
                                                 + "        <name>espresso</name>"
                                                 + "        <quantity>3</quantity>"
                                                 + "        <size>medium</size>"
                                                 + "    </item>"
                                                 + "    </items>"
                                                 + "</order>");
                                     });

      Assert.That(createdResponse.StatusCode, Is.EqualTo(HttpStatusCode.Created));
      Assert.That(createdResponse.Headers.Keys, Contains.Item("Location"));

      var orderPath = createdResponse.Headers["Location"].Remove(0, 12);

      var getOrderResponse = app.Get(orderPath);
      Assert.That(getOrderResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK), string.Format("order at {0} not found", orderPath));

      var deletedResponse = app.Delete(orderPath);
      Assert.That(deletedResponse.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));

      var getDeletedOrderResponse = app.Get(orderPath);
      Assert.That(getDeletedOrderResponse.StatusCode, Is.EqualTo(HttpStatusCode.MovedPermanently));
    }
  }
}
