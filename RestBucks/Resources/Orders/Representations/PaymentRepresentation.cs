using System.Xml.Serialization;

namespace RestBucks.Resources.Orders.Representations
{
    [XmlRoot("payment", Namespace = "http://restbuckson.net")]
    public class PaymentRepresentation
    {
        [XmlElement("card-number")]
        public string CardNumber { get; set; }

        [XmlElement("card-owner")]
        public string CardOwner { get; set; }
    }
}