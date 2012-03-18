namespace RestBucks.Resources.Orders.Representations
{
  using System.Xml.Serialization;

  [XmlRoot("payment", Namespace = "http://restbuckson.net")]
  public class PaymentRepresentation
  {
    [XmlElement("card-number")]
    public string CardNumber { get; set; }

    [XmlElement("card-owner")]
    public string CardOwner { get; set; }
  }
}