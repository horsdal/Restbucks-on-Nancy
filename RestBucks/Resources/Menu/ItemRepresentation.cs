using System.Xml.Serialization;

namespace RestBucks.Resources.Products
{
    [XmlRoot("item")]
    public class ItemRepresentation
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
    }
}