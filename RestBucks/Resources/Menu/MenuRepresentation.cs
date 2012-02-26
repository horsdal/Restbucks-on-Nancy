using System.Xml.Serialization;

namespace RestBucks.Resources.Products
{
    [XmlRoot("menu")]
    public class MenuRepresentation
    {
        [XmlElement("item")]
        public ItemRepresentation[] Items { get; set; }
    }
}