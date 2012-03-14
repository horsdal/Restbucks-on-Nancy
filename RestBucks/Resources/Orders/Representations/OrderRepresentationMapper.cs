using System.Collections.Generic;
using System.Linq;
using RestBucks.Domain;
using RestBucks.Infrastructure;
using RestBucks.Infrastructure.Linking;

namespace RestBucks.Resources.Orders.Representations
{
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
                                               OrderResourceHandler.BaseResoureUriTemplate,
                                               new {orderId = order.Id}),
                         baseUri + "docs/order-get.htm",
                         MediaTypes.Default);

      var update = new Link(linker.GetUri<OrderResourceHandler>(r => r.Update(0, null), new {orderId = order.Id}),
                            baseUri + "docs/order-update.htm",
                            MediaTypes.Default);
      var cancel = new Link(linker.GetUri<OrderResourceHandler>(r => r.Cancel(0), new {orderId = order.Id}),
                            baseUri + "docs/order-cancel.htm",
                            MediaTypes.Default);
      var pay = new Link(linker.GetUri<OrderResourceHandler>(r => r.Pay(0, null), new {orderId = order.Id}),
                         baseUri + "docs/order-pay.htm",
                         MediaTypes.Default);
      var receipt = new Link(linker.GetUri<OrderResourceHandler>(r => r.Receipt(0), new {orderId = order.Id}),
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