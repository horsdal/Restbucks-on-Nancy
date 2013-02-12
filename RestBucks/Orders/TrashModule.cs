namespace RestBucks.Orders
{
  using System.Linq;
  using Data;
  using Domain;
  using Infrastructure;
  using Nancy;
  using Representations;

  public class TrashModule : NancyModule
  {
    private readonly IRepository<Order> orderRepository;
    public const string GetCancelledPath = "/order/{orderId}";
    public const string path = "/trash";

    public TrashModule(IRepository<Order> orderRepository)
      : base(path)
    {
      this.orderRepository = orderRepository;

      Get[GetCancelledPath] = parameters => GetCanceled((int) parameters.orderId);
    }

    private object GetCanceled(int orderId)
    {
      var order = orderRepository.Retrieve(o => o.Id == orderId && o.Status == OrderStatus.Canceled)
        .FirstOrDefault();

      if (order == null)
      {
          return HttpStatusCode.NotFound;
      }

      return Negotiate.WithModel(OrderRepresentationMapper.Map(order, Request.BaseUri())).WithCacheHeaders(order);
    }
  }
}