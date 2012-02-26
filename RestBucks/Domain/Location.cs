using System.Xml.Serialization;

namespace RestBucks.Domain
{
    public enum Location
    {
        [XmlEnum("takeAway")]
        TakeAway,
        [XmlEnum("inShop")]
        InShop
    }
}