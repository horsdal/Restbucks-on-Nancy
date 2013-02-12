namespace RestBucks.Orders
{
  using System;
  using Data;
  using Domain;
  using Infrastructure;
  using Infrastructure.Domain.BaseClass;
  using Infrastructure.Linking;
  using Nancy;
  using Nancy.ModelBinding;
  using Products.Domain;
  using Representations;

  public class OrdersResourceModule : NancyModule
  {
    private readonly IRepository<Product> productRepository;
    private readonly IRepository<Order> orderRepository;
    private Response errorResponse;

    public OrdersResourceModule(IRepository<Product> productRepository,
                                 IRepository<Order> orderRepository)
      : base("/orders")
    {
      this.productRepository = productRepository;
      this.orderRepository = orderRepository;

      Post["/"] = _ => HandlePost(this.Bind<OrderRepresentation>());
    }

    private Response HandlePost(OrderRepresentation orderRepresentation)
    {
      var order = TryBuildOrder(orderRepresentation);
      if (!order.IsValid())
        return InvalidOrderResponse(order);

      orderRepository.MakePersistent(order);
      return Created(order);
    }

    private Order TryBuildOrder(OrderRepresentation orderRepresentation)
    {
      var order = new Order { Date = DateTime.Today, Location = orderRepresentation.Location };
      foreach (var requestedItem in orderRepresentation.Items)
        if (!TryAddOrderItem(order, requestedItem)) 
          break;
      
      return order;
    }

    private bool TryAddOrderItem(Order order, OrderItemRepresentation requestedItem)
    {
      var product = TryFindProduct(requestedItem);
      if (product == null)
        return false;

      var orderItem = new OrderItem(product, requestedItem.Quantity, product.Price, requestedItem.Preferences);
      order.AddItem(orderItem);
      return true;
    }

    private Product TryFindProduct(OrderItemRepresentation requestedItem)
    {
      var product = productRepository.GetByName(requestedItem.Name);
      if (product == null)
        errorResponse = Response.BadRequest(string.Format("We don't offer {0}", requestedItem.Name));
      return product;
    }

    private Response InvalidOrderResponse(Order order)
    {
      if (errorResponse != null)
        return errorResponse;

      var content = string.Join("\n", order.GetErrorMessages());
      return Response.BadRequest("Invalid entities values", content);
    }

    private Response Created(Order order)
    {
      var uri = new ResourceLinker(Request.BaseUri()).BuildUriString(
        OrderResourceModule.Path,
        OrderResourceModule.SlashOrderId,
        new {orderId = order.Id});
      
      return Response.Created(uri);
    }
  }
}