namespace RestBucks.Resources.Orders.Representations
{
  using System.Collections.Generic;
  using System.Linq;

  using Domain;

  using Infrastructure;
  using Infrastructure.Linking;

  public static class OrderRepresentationMapper
  {
    public static OrderRepresentation Map(Order order)
    {
      return new OrderRepresentation
             {
               Cost = order.Total,
               Status = order.Status,
               Location = order.Location,
               Items = order.Items.Select(i => new OrderItemRepresentation
                                               {
                                                 Name = i.Product.Name,
                                                 Preferences = i.Preferences.ToDictionary(p => p.Key, p => p.Value),
                                                 Quantity = i.Quantity
                                               }).ToList(),
               Links = GetLinks(order).ToList()
             };

    }

    private static IEnumerable<Link> GetLinks(Order order)
    {
      var baseUri = new UriSegment(BaseAddress.Current);
      var linker = new ResourceLinker();

      var get = new Link(linker.BuildUriString(OrderResourceHandler.Path,
                                               OrderResourceHandler.SlashOrderId,
                                               new {orderId = order.Id}),
                         baseUri + "docs/order-get.htm",
                         MediaTypes.Default);

      var update = new Link(linker.BuildUriString(OrderResourceHandler.Path,
                                                  OrderResourceHandler.SlashOrderId,
                                                  new {orderId = order.Id}),
                            baseUri + "docs/order-update.htm",
                            MediaTypes.Default);

      var cancel = new Link(linker.BuildUriString(OrderResourceHandler.Path,
                                                  OrderResourceHandler.SlashOrderId,
                                                  new {orderId = order.Id}),
                            baseUri + "docs/order-cancel.htm",
                            MediaTypes.Default);

      var pay = new Link(linker.BuildUriString(OrderResourceHandler.Path,
                                               OrderResourceHandler.PaymentPath,
                                               new {orderId = order.Id}),
                         baseUri + "docs/order-pay.htm",
                         MediaTypes.Default);

      var receipt = new Link(linker.BuildUriString(OrderResourceHandler.Path,
                                                   OrderResourceHandler.ReceiptPath,
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