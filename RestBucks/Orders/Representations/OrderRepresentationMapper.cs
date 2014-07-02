namespace RestBucks.Orders.Representations
{
  using System.Collections.Generic;
  using System.Linq;
  using Domain;
  using Infrastructure;
  using Infrastructure.Linking;
  using Infrastructure.Resources;
  using Nancy;
  using Nancy.Routing;

  public static class OrderRepresentationMapper
  {
    public static OrderRepresentation Map(Order order, ResourceLinker resourceLinker, NancyContext context)
    {
      return new OrderRepresentation(order)
      {
        Links = GetLinks(order, resourceLinker, context).ToList()
      };
    }

    private static IEnumerable<Link> GetLinks(Order order, ResourceLinker linker, NancyContext context)
    {
      var get = new Link(
        linker.BuildUriString(context, "ReadOrder", new {orderId = order.Id}),
        context.Request.BaseUri() + "/docs/order-get.htm",
        MediaTypes.Default);

      var update = new Link(
        linker.BuildUriString(context, "UpdateOrder", new { orderId = order.Id}),
        context.Request.BaseUri() + "/docs/order-update.htm",
        MediaTypes.Default);

      var cancel = new Link(
        linker.BuildUriString(context, "CancelOrder", new { orderId = order.Id}),
        context.Request.BaseUri() + "/docs/order-cancel.htm",
        MediaTypes.Default);

      var pay = new Link(linker.BuildUriString(context, "PayOrder", new {orderId = order.Id}),
        context.Request.BaseUri() + "/docs/order-pay.htm",
        MediaTypes.Default);

      switch (order.Status)
      {
        case OrderStatus.Unpaid:
          yield return get;
          yield return update;
          yield return cancel;
          yield return pay;
          break;
        case OrderStatus.Paid:
        case OrderStatus.Delivered:
          yield return get;
          break;
        case OrderStatus.Ready:
          break;
        case OrderStatus.Canceled:
          yield break;
        default:
          yield break;
      }
    }
  }
}