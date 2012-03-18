namespace RestBucks.Resources.Orders
{
  using System.Linq;

  using Nancy;

  using Data;
  using Domain;
  using Representations;

  public class TrashHandler : NancyModule
  {
    private readonly IRepository<Order> orderRepository;
    public const string GetCancelledPath = "/order/{orderId}";
    public const string path = "/trash";

    public TrashHandler(IRepository<Order> orderRepository)
      : base(path)
    {
      this.orderRepository = orderRepository;

      Get[GetCancelledPath] = parameters => GetCanceled((int) parameters.orderId);
    }

    public Response GetCanceled(int orderId)
    {
      var order = orderRepository.Retrieve(o => o.Id == orderId && o.Status == OrderStatus.Canceled)
        .FirstOrDefault();

      return order == null
               ? HttpStatusCode.NotFound
               : Response.AsXml(OrderRepresentationMapper.Map(order));
    }
  }
}