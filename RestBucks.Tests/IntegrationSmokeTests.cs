namespace RestBucks.Tests
{
  using NUnit.Framework;

  using Nancy;
  using Nancy.Testing;

  using RestBucks;
  using RestBucks.Resources.Orders.Representations;

  using Util;

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

      var createdResponse = CreatedOrder(app);

      Assert.That(createdResponse.StatusCode, Is.EqualTo(HttpStatusCode.Created));
      Assert.That(createdResponse.Headers.Keys, Contains.Item("Location"));

      var orderPath = GetOrderPath(createdResponse);

      var getOrderResponse = app.Get(orderPath);
      Assert.That(getOrderResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK), string.Format("order at {0} not found", orderPath));

      var deletedResponse = app.Delete(orderPath);
      Assert.That(deletedResponse.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));

      var getDeletedOrderResponse = app.Get(orderPath);
      Assert.That(getDeletedOrderResponse.StatusCode, Is.EqualTo(HttpStatusCode.MovedPermanently));
    }

    private static string GetOrderPath(BrowserResponse createdResponse)
    {
      var orderPath = createdResponse.Headers["Location"].Remove(0, 12);
      return orderPath;
    }

    private static BrowserResponse CreatedOrder(Browser app)
    {
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
      return createdResponse;
    }

    [Test]
    public void AppReturnsBadRequestWhenCancelingPaidOrder()
    {
      var app = new Browser(new Bootstrapper());

      var createdResponse = CreatedOrder(app);
      var orderPath = GetOrderPath(createdResponse);

      var paymentResponse = app.Post(orderPath + "/payment/",
                                     with =>
                                     {
                                       with.HttpRequest();
                                       var xmlString = new PaymentRepresentation {CardNumber = "321", CardOwner = "Jose"}.ToXmlString();
                                       with.Body(xmlString);
                                     });
      Assert.That(paymentResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));

      var cancelResponse = app.Delete(orderPath);
      Assert.That(cancelResponse.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test, Ignore]
    public void IndexPageIsAvailable()
    {
      var app = new Browser(new Bootstrapper());

      var response = app.Get("/");

      Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
      Assert.That(response.Context.Response.ContentType, Is.EqualTo("text/html"));
      response.Body["title"].ShouldContain("Restbuck API home page");
    }

    [Test, Ignore]
    public void DocsAreAvailable()
    {
      var app = new Browser(new Bootstrapper());

      var response = app.Get("/docs/order-get.htm/");

      Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
      Assert.That(response.Context.Response.ContentType, Is.EqualTo("text/html"));      
    }
  }
}
