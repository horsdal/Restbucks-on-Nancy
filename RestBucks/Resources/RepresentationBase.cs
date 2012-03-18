namespace RestBucks.Resources
{
  using System.Collections.Generic;
  using System.Xml.Serialization;

  public class RepresentationBase
  {
    public RepresentationBase()
    {
      Links = new List<Link>();
    }

    [XmlArray(ElementName = "links"), XmlArrayItem(ElementName = "link")]
    public List<Link> Links { get; set; }
  }
}