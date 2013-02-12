namespace RestBucks.Menu
{
  using System.Xml.Serialization;

  [XmlRoot("menu")]
  public class MenuRepresentation
  {
    [XmlElement("item")]
    public ItemRepresentation[] Items { get; set; }
  }
}