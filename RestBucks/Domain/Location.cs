namespace RestBucks.Domain
{
  using System.Xml.Serialization;

  public enum Location
  {
    [XmlEnum("takeAway")] TakeAway,
    [XmlEnum("inShop")] InShop
  }
}