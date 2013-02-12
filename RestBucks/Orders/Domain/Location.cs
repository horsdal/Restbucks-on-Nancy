namespace RestBucks.Orders.Domain
{
  using System.Xml.Serialization;

  public enum Location
  {
    [XmlEnum("takeAway")] TakeAway,
    [XmlEnum("inShop")] InShop
  }
}