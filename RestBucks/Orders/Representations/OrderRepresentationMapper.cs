namespace RestBucks.Orders.Representations
{
  using System.Collections.Generic;
  using System.Linq;
  using Domain;
  using Infrastructure;
  using Infrastructure.Linking;
  using Infrastructure.Resources;

  public static class OrderRepresentationMapper
  {
    public static OrderRepresentation Map(Order order, string baseAddress)
    {
      return new OrderRepresentation(order)
             {
               Links = GetLinks(order, baseAddress).ToList()
             };

    }

    private static IEnumerable<Link> GetLinks(Order order, string baseAddress)
    {
      var baseUri = new UriSegment(baseAddress);
      var linker = new ResourceLinker(baseAddress);

      var get = new Link(linker.BuildUriString(OrderResourceModule.Path,
                                               OrderResourceModule.SlashOrderId,
                                               new {orderId = order.Id}),
                         baseUri + "docs/order-get.htm",
                         MediaTypes.Default);

      var update = new Link(linker.BuildUriString(OrderResourceModule.Path,
                                                  OrderResourceModule.SlashOrderId,
                                                  new {orderId = order.Id}),
                            baseUri + "docs/order-update.htm",
                            MediaTypes.Default);

      var cancel = new Link(linker.BuildUriString(OrderResourceModule.Path,
                                                  OrderResourceModule.SlashOrderId,
                                                  new {orderId = order.Id}),
                            baseUri + "docs/order-cancel.htm",
                            MediaTypes.Default);

      var pay = new Link(linker.BuildUriString(OrderResourceModule.Path,
                                               OrderResourceModule.PaymentPath,
                                               new {orderId = order.Id}),
                         baseUri + "docs/order-pay.htm",
                         MediaTypes.Default);

      var receipt = new Link(linker.BuildUriString(OrderResourceModule.Path,
                                                   OrderResourceModule.ReceiptPath,
                                                   new {orderId = order.Id}),
                             baseUri + "docs/receipt-coffee.htm",
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
          yield return receipt;
          break;
        case OrderStatus.Canceled:
          yield break;
        default:
          yield break;
      }
    }
  }
}