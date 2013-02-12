namespace RestBucks.Orders.Domain
{
  using System.Xml.Serialization;

  public enum OrderStatus
  {
    [XmlEnum("orderCreated")] OrderCreated,
    [XmlEnum("unpaid")] Unpaid,
    [XmlEnum("paid")] Paid,
    [XmlEnum("ready")] Ready,
    [XmlEnum("canceled")] Canceled,
    [XmlEnum("delivered")] Delivered
  }
}