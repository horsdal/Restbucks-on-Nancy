using System.Net;
using Microsoft.ApplicationServer.Http;
using NUnit.Framework;
using RestBucks.Domain;
using RestBucks.Infrastructure.Linking;
using RestBucks.Resources.Orders;
using RestBucks.Resources.Orders.Representations;
using RestBucks.Tests.Util;
using SharpTestsEx;

namespace RestBucks.Tests.Resources
{
    [TestFixture]
    public class WhenUserPayAnOrder
    {
       private readonly IResourceLinker resourceLinker = new ResourceLinker();
       [Test]
       public void WhenOrderDoesNotExist_ThenReturn404()
       {
           var handler = new OrderResourceHandler(new RepositoryStub<Order>(), resourceLinker);
           var response = handler.Pay(123, new PaymentRepresentation());
           response.StatusCode.Should().Be.EqualTo(HttpStatusCode.NotFound);
       }

       [Test]
       public void WhenOrderExist_ThenCancel()
       {
           var order = new Order{Id = 123};
           var handler = new OrderResourceHandler(new RepositoryStub<Order>(order), resourceLinker);
           handler.Pay(123, new PaymentRepresentation{CardNumber = "123", CardOwner = "Jose"});
           order.Satisfy(o => o.Status == OrderStatus.Paid
                              && o.Payment.CardOwner == "Jose"
                              && o.Payment.CreditCardNumber == "123");
       }
    }
}