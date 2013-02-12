namespace RestBucks.Tests.Resources
{
  using System;
  using System.Linq;

  using Moq;

  using NUnit.Framework;

  using Nancy;
  using Nancy.Testing;
  using Orders;
  using Orders.Domain;
  using Orders.Representations;
  using RestBucks.Data;
  using RestBucks.Infrastructure.Linking;
  using Util;

  using SharpTestsEx;

  [TestFixture]
  public class WhenUserCreatesAnOrder : ResourceHandlerTestBase
  {
    private readonly ResourceLinker resourceLinker = new ResourceLinker("http://bogus/");

    [Test]
    public void WhenAProductDoesNotExist_ThenReturn400AndTheProperREasonPhrase()
    {
      var appProxy = CreateAppProxy();
      var orderRepresentation = new OrderRepresentation()
                                {
                                  Items = {new OrderItemRepresentation() {Name = "beer"}}
                                };

      var result =
        appProxy.Post("/orders/",
                      with =>
                      {
                        with.Header("Content-Type", "application/xml");
                        with.Body(orderRepresentation.ToXmlString());
                      });


      result.StatusCode.Should().Be.EqualTo(HttpStatusCode.BadRequest);
      result.Headers["ReasonPhrase"].Should().Be.EqualTo("We don't offer beer");
    }

    [Test]
    public void WhenItemHasQuantity0_ThenReturn400AndTheProperREasonPhrase()
    {
      var appProxy = CreateAppProxy();
      var orderRepresentation = new OrderRepresentation()
                                {
                                  Items = {new OrderItemRepresentation() {Name = "latte", Quantity = 0}}
                                };
      
      // act
      var result =
        appProxy.Post("/orders/",
                      with =>
                      {
                        with.Header("Content-Type", "application/xml");
                        with.Body(orderRepresentation.ToXmlString());
                      });

      // assert
      result.StatusCode.Should().Be.EqualTo(HttpStatusCode.BadRequest);
      result.Headers["ReasonPhrase"].Should().Be.EqualTo("Invalid entities values");
      result.Body.AsString().Should().Be.EqualTo("Item 0: Quantity should be greater than 0.");
    }

    [Test]
    public void WhenOrderIsOk_ThenInsertANewOrderWithTheProductsAndPrice()
    {
      var orderRepository = new RepositoryStub<Order>();
      var appProxy = CreateAppProxy(orderRepository);
      var orderRepresentation = new OrderRepresentation() {Items = {new OrderItemRepresentation() {Name = "latte", Quantity = 1}}};

      //act
      appProxy.Post("/orders/",
                      with =>
                      {
                        with.Header("Content-Type", "application/xml");
                        with.Body(orderRepresentation.ToXmlString());
                      });

      // assert
      var order = orderRepository.RetrieveAll().First();
      order.Satisfy(_ => _.Items.Any(i => i.Product == latte && i.UnitPrice == 2.5m && i.Quantity == 1));
    }

    [Test]
    public void WhenOrderIsOk_ThenInsertANewOrderWithTheDateTime()
    {
      var orderRepository = new RepositoryStub<Order>();
      var appProxy = CreateAppProxy(orderRepository);
      var orderRepresentation = new OrderRepresentation() {Items = {new OrderItemRepresentation() {Name = "latte", Quantity = 1}}};

      //act
      var result = appProxy.Post("/orders/",
                                 with =>
                                 {
                                   with.Header("Content-Type", "application/xml");
                                   with.Body(orderRepresentation.ToXmlString());
                                 });

      var order = orderRepository.RetrieveAll().First();
      order.Date.Should().Be.EqualTo(DateTime.Today);
    }

    [Test]
    public void WhenOrderIsOk_ThenInsertANewOrderWithTheLocationInfo()
    {
      var orderRepository = new RepositoryStub<Order>();
      var appProxy = CreateAppProxy(orderRepository);
      var orderRepresentation = new OrderRepresentation()
                                {
                                  Location = Location.InShop,
                                  Items = {new OrderItemRepresentation() {Name = "latte", Quantity = 1}}
                                };

      //act
      var result = appProxy.Post("/orders/",
                                 with =>
                                 {
                                   with.Header("Content-Type", "application/xml");
                                   with.Body(orderRepresentation.ToXmlString());
                                 });

      var order = orderRepository.RetrieveAll().First();
      order.Location.Should().Be.EqualTo(Location.InShop);
    }

    [Test]
    public void WhenOrderIsOk_ThenResponseHasStatus201AndLocation()
    {
      var orderRepository = new Mock<IRepository<Order>>();
      orderRepository
        .Setup(or => or.MakePersistent(It.IsAny<Order[]>()))
        .Callback<Order[]>(o => o.First().Id = 123);

      var expectedUriToTheNewOrder =
        resourceLinker.BuildUriString("/order/", OrderResourceModule.SlashOrderId,new { orderId = "123" });

      var appProxy = CreateAppProxy(orderRepository.Object);
      var orderRepresentation = new OrderRepresentation()
                                {
                                  Items = {new OrderItemRepresentation() {Name = "latte", Quantity = 1}}
                                };
      // act
      var result = appProxy.Post("/orders/",
                      with =>
                      {
                        with.Header("Content-Type", "application/xml");
                        with.Body(orderRepresentation.ToXmlString());
                      });


      result.StatusCode.Should().Be.EqualTo(HttpStatusCode.Created);
      result.Headers["Location"].Should().Be.EqualTo(expectedUriToTheNewOrder);
    }
  }
}