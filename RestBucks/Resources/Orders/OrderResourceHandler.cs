using System;
using System.Net.Http;
using System.ServiceModel;
using System.ServiceModel.Web;

using RestBucks.Data;
using RestBucks.Domain;
using RestBucks.Infrastructure;
using RestBucks.Infrastructure.Linking;
using RestBucks.Resources.Orders.Representations;

namespace RestBucks.Resources.Orders
{
  using Nancy;
  using Nancy.ModelBinding;

  [ServiceContract, WithUriPrefix("order")]
  public class OrderResourceHandler : NancyModule
  {
    private readonly IRepository<Order> orderRepository;
    private readonly IResourceLinker linker;

    public static string SlashOrderId = "/{orderId}/";
    public static string Path = "/order";

    public OrderResourceHandler(IRepository<Order> orderRepository, IResourceLinker linker) 
      : base(Path)
    {
      this.orderRepository = orderRepository;
      this.linker = linker;

      Get[SlashOrderId] = parameters => GetHandler((int) parameters.orderId);
      Post[SlashOrderId] = parameter => Update((int)parameter.orderId, this.Bind<OrderRepresentation>());
    }

    [WebGet(UriTemplate = "{orderId}")]
    public Response GetHandler(int orderId)
    {
      var order = orderRepository.GetById(orderId);
      if (order == null) 
        return HttpStatusCode.NotFound;

      if (order.Status == OrderStatus.Canceled)
        return Response.MovedTo(linker.GetUri<TrashHandler>(rh => rh.GetCanceled(0), new {orderId}));

      if (Request.IsNotModified(order)) 
        return Response.NotModified();

      return Response.AsXml(OrderRepresentationMapper.Map(order))
                     .AddCacheHeaders(order);
    }

    [WebInvoke(UriTemplate = "{orderId}", Method = "POST")]
    public Response Update(int orderId, OrderRepresentation orderRepresentation)
    {
      var order = orderRepository.GetById(orderId);
      if (order == null)
        return HttpStatusCode.NotFound;

      order.Location = orderRepresentation.Location;
      return HttpStatusCode.NoContent;
    }

    [WebInvoke(UriTemplate = "{orderId}", Method = "DELETE")]
    public HttpResponseMessage Cancel(int orderId)
    {
      var order = orderRepository.GetById(orderId);
      if (order == null) return Responses.NotFound();
      order.Cancel("canceled from the rest interface");
      return Responses.NoContent();
    }

    [WebInvoke(UriTemplate = "{orderId}/payment", Method = "POST")]
    public HttpResponseMessage Pay(int orderId, PaymentRepresentation paymentArgs)
    {
      var order = orderRepository.GetById(orderId);
      if (order == null) return Responses.NotFound();
      order.Pay(paymentArgs.CardNumber, paymentArgs.CardOwner);
      return Responses.Ok();
    }

    [WebGet(UriTemplate = "{orderId}/receipt")]
    public HttpResponseMessage Receipt(int orderId)
    {
      //return Get(orderId);
      throw new NotImplementedException();
    }
  }
}