namespace RestBucks.Resources.Orders
{
  using Nancy;
  using Nancy.ModelBinding;

  using System;

  using Data;

  using Domain;

  using Infrastructure;
  using Infrastructure.Linking;

  using Representations;

  public class OrdersResourceHandler : NancyModule
  {
    private readonly IRepository<Product> productRepository;
    private readonly IRepository<Order> orderRepository;
    private readonly IResourceLinker resourceLinker;

    public OrdersResourceHandler(IRepository<Product> productRepository,
                                 IRepository<Order> orderRepository,
                                 IResourceLinker resourceLinker)
      : base("/orders")
    {
      this.productRepository = productRepository;
      this.orderRepository = orderRepository;
      this.resourceLinker = resourceLinker;

      Post["/"] = _ => HandlePost(this.Bind<OrderRepresentation>());
    }

    private Response HandlePost(OrderRepresentation orderRepresentation)
    {
      Response errorResponse;
      Order order;
      if (!CreateAndValidateOrder(orderRepresentation, out errorResponse, out order)) return errorResponse;

      orderRepository.MakePersistent(order);

      return Created(order);
    }

    private bool CreateAndValidateOrder(OrderRepresentation orderRepresentation, out Response errorResponse, out Order order)
    {
      if (!TryBuildOrder(orderRepresentation, out errorResponse, out order)) return false;

      if (!order.IsValid()) return InvalidOrderResponse(out errorResponse, order);

      return true;
    }

    private bool TryBuildOrder(OrderRepresentation orderRepresentation, out Response errorResponse, out Order order)
    {
      errorResponse = null;

      order = new Order { Date = DateTime.Today, Location = orderRepresentation.Location };
      foreach (var requestedItem in orderRepresentation.Items)
        if (!TryAddOrderItem(order, requestedItem, out errorResponse)) return false;
      
      return true;
    }

    private bool TryAddOrderItem(Order order, OrderItemRepresentation requestedItem, out Response errorResponse)
    {
      Product product;
      if (!TryFindProduct(requestedItem, out product, out errorResponse)) return false;

      var orderItem = new OrderItem(product, requestedItem.Quantity, product.Price, requestedItem.Preferences);
      order.AddItem(orderItem);
      return true;
    }

    private bool TryFindProduct(OrderItemRepresentation requestedItem, out Product product, out Response errorResponse)
    {
      errorResponse = null;
      product = productRepository.GetByName(requestedItem.Name);
      if (product == null)
      {
        errorResponse = Response.BadRequest(string.Format("We don't offer {0}", requestedItem.Name));
        return false;
      }
      return true;
    }

    private bool InvalidOrderResponse(out Response errorResponse, Order order)
    {
      var content = string.Join("\n", order.GetErrorMessages());
      {
        errorResponse = Response.BadRequest("Invalid entities values", content);
        return false;
      }
    }

    private Response Created(Order order)
    {
      var uri = resourceLinker.GetUri<OrderResourceHandler>(
        orderResource => orderResource.Get(0, null),
        new {orderId = order.Id});
      return Response.Created(uri);
    }
  }
}