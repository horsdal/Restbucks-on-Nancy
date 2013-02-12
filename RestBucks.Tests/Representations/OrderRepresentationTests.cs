namespace RestBucks.Tests.Representations
{
  using System;
  using System.Linq;

  using NUnit.Framework;
  using Orders.Domain;
  using Orders.Representations;
  using RestBucks.Tests.Util;

  using SharpTestsEx;

  [TestFixture]
  public class OrderRepresentationTests
  {
    [Test]
    public void SerializeOrder()
    {
      var orderRepresentation = new OrderRepresentation
                                {
                                  Cost = 100.4m,
                                  Location = Location.InShop,
                                  Items =
                                    {
                                      new OrderItemRepresentation
                                      {
                                        Name = "latte",
                                        Preferences =
                                          {
                                            {"size", "large"},
                                            {"milk", "skim"}
                                          }
                                      }
                                    }
                                };

      Assert.DoesNotThrow((() => orderRepresentation.ToXmlString()));
    }

    [Test]
    public void CanDeserialize()
    {
      var xml =
        @"<?xml version=""1.0""?>
<order xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns=""http://restbuckson.net"">
  <location>inShop</location>
  <cost>100.4</cost>
  <items>
    <item>
      <name>latte</name>
      <quantity>0</quantity>
      <size>large</size>
      <milk>skim</milk>
    </item>
  </items>
</order>";
      var representation = XmlUtil.FromXmlString<OrderRepresentation>(xml);
      representation.Satisfy(r =>
                             r.Items.Any(i => i.Name == "latte"
                                              && i.Preferences.Any(p => p.Key == "size")
                                              && i.Preferences.Any(p => p.Key == "milk")));
    }
  }
}