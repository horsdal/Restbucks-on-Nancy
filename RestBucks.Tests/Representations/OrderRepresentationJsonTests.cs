namespace RestBucks.Tests.Representations
{
  using NUnit.Framework;

  using RestBucks.Domain;
  using RestBucks.Resources.Orders.Representations;

  using Util;

  [TestFixture]
  public class OrderRepresentationJsonTests
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

      var json = orderRepresentation.ToJsonString();

      var expected =
        "{\"Location\":\"InShop\",\"Cost\":100.4,\"Items\":[{\"Name\":\"latte\",\"Quantity\":0,\"Preferences\":{\"size\":\"large\",\"milk\":\"skim\"}}],\"Status\":\"OrderCreated\",\"Links\":[]}";

      Assert.AreEqual(expected, json);
    }
  }
}