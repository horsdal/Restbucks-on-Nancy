namespace RestBucks.Tests.Resources
{
  using System;
  using System.Linq;
  using System.Net;

  using Moq;

  using NUnit.Framework;

  using RestBucks.Data;
  using RestBucks.Domain;
  using RestBucks.Infrastructure.Linking;
  using RestBucks.Resources.Orders;
  using RestBucks.Resources.Orders.Representations;
  using RestBucks.Tests.Util;

  using SharpTestsEx;

  [TestFixture]
  public class WhenUserCreatesAnOrder
  {
    private readonly Product latte = new Product
                                     {
                                       Name = "latte",
                                       Price = 2.5m,
                                       Customizations =
                                         {
                                           new Customization
                                           {
                                             Name = "size",
                                             PossibleValues = {"small", "medium"}
                                           }
                                         }
                                     };

    private readonly IResourceLinker resourceLinker = new ResourceLinker();

    public OrdersResourceHandler CreateResourceHandler(
      IRepository<Order> orderRepository = null,
      IRepository<Product> productRepository = null)
    {
      var defaultProductRepository = new RepositoryStub<Product>(
        latte, new Product {Name = "Other", Price = 3.6m});

      return new OrdersResourceHandler(
        defaultProductRepository ?? new RepositoryStub<Product>(),
        orderRepository ?? new RepositoryStub<Order>(),
        resourceLinker);
    }

    [Test]
    public void WhenAProductDoesNotExist_ThenReturn400AndTheProperREasonPhrase()
    {
      var resourceHandler = CreateResourceHandler();
      var orderRepresentation = new OrderRepresentation
                                {
                                  Items = {new OrderItemRepresentation {Name = "beer"}}
                                };

      var result = resourceHandler.Create(orderRepresentation);

      result.Satisfy(rm => rm.StatusCode == HttpStatusCode.BadRequest
                           && rm.ReasonPhrase == "We don't offer beer");
    }

    [Test]
    public void WhenItemHasQuantity0_ThenReturn400AndTheProperREasonPhrase()
    {
      var service = CreateResourceHandler();
      var orderRepresentation = new OrderRepresentation
                                {
                                  Items = {new OrderItemRepresentation {Name = "latte", Quantity = 0}}
                                };

      var response = service.Create(orderRepresentation);

      //NOTE: I am not sure if the proper response is NotFound or BadRequest. It is not clear in the book.
      response.Satisfy(rm => rm.StatusCode == HttpStatusCode.BadRequest
                             && rm.Content.ToStringContent() == "Item 0: Quantity should be greater than 0.");
    }

    [Test]
    public void WhenOrderIsOk_ThenInsertANewOrderWithTheProductsAndPrice()
    {
      var orderRepository = new RepositoryStub<Order>();
      var handler = CreateResourceHandler(orderRepository);
      var orderRepresentation = new OrderRepresentation
                                {Items = {new OrderItemRepresentation {Name = "latte", Quantity = 1}}};

      //act
      handler.Create(orderRepresentation);

      var order = orderRepository.RetrieveAll().First();
      order.Satisfy(_ => _.Items.Any(i => i.Product == latte && i.UnitPrice == 2.5m && i.Quantity == 1));
    }

    [Test]
    public void WhenOrderIsOk_ThenInsertANewOrderWithTheDateTime()
    {
      var orderRepository = new RepositoryStub<Order>();
      var handler = CreateResourceHandler(orderRepository);
      var orderRepresentation = new OrderRepresentation
                                {Items = {new OrderItemRepresentation {Name = "latte", Quantity = 1}}};

      //act
      handler.Create(orderRepresentation);

      var order = orderRepository.RetrieveAll().First();
      order.Date.Should().Be.EqualTo(DateTime.Today);
    }

    [Test]
    public void WhenOrderIsOk_ThenInsertANewOrderWithTheLocationInfo()
    {
      var orderRepository = new RepositoryStub<Order>();
      var handler = CreateResourceHandler(orderRepository);
      var orderRepresentation = new OrderRepresentation
                                {
                                  Location = Location.InShop,
                                  Items = {new OrderItemRepresentation {Name = "latte", Quantity = 1}}
                                };

      //act
      handler.Create(orderRepresentation);

      var order = orderRepository.RetrieveAll().First();
      order.Location.Should().Be.EqualTo(Location.InShop);
    }

    [Test]
    public void WhenOrderIsOk_ThenResponseHasStatus201AndLocation()
    {
      var orderRepository = new Mock<IRepository<Order>>();
      orderRepository.Setup(or => or.MakePersistent(It.IsAny<Order[]>()))
        .Callback<Order[]>(o => o.First().Id = 123);

      var handler = CreateResourceHandler(orderRepository.Object);


      //act
      var result =
        handler.Create(new OrderRepresentation {Items = {new OrderItemRepresentation {Name = "latte", Quantity = 1}}});

      var expectedUriToTheNewOrder =
        resourceLinker.GetUri<OrderResourceHandler>(or => or.Get(0, null), new {orderId = "123"});

      result.Satisfy(r => r.StatusCode == HttpStatusCode.Created
                          && r.Headers.Location.ToString() == expectedUriToTheNewOrder);
    }
  }
}