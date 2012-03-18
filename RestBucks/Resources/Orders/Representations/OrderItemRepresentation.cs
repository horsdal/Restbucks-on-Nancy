namespace RestBucks.Resources.Orders.Representations
{
  using System.Collections.Generic;
  using System.Linq;
  using System.Xml;
  using System.Xml.Linq;
  using System.Xml.Schema;
  using System.Xml.Serialization;

  public class OrderItemRepresentation : IXmlSerializable
  {
    public OrderItemRepresentation()
    {
      Preferences = new Dictionary<string, string>();
    }

    public string Name { get; set; }
    public int Quantity { get; set; }

    [XmlIgnore]
    public IDictionary<string, string> Preferences { get; set; }

    public XmlSchema GetSchema()
    {
      return null;
    }

    public void ReadXml(XmlReader reader)
    {
      var standardNames = new[] {"name", "quantity"};
      var node = (XElement) XNode.ReadFrom(reader);

      var nameElement = node.Element(XName.Get("name", "http://restbuckson.net"));
      if (nameElement != null) Name = nameElement.Value;

      Quantity = (int) node.Element(XName.Get("quantity", "http://restbuckson.net"));
      Preferences = node.Elements()
        .Where(x => !standardNames.Contains(x.Name.LocalName))
        .ToDictionary(x => x.Name.LocalName, x => x.Value);
    }

    public void WriteXml(XmlWriter writer)
    {
      writer.WriteElementString("name", Name);

      writer.WriteStartElement("quantity");
      writer.WriteValue(Quantity);
      writer.WriteEndElement();

      foreach (var preference in Preferences)
      {
        writer.WriteElementString(preference.Key, preference.Value);
      }

    }
  }
}