using System.Net;
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
    public class WhenUserUpdateAnOrder
    {
        private readonly IResourceLinker resourceLinker = new ResourceLinker();

        [Test]
        public void WhenOrderDoesNotExist_ThenReturn404()
        {
            var handler = new OrderResourceHandler(new RepositoryStub<Order>(), resourceLinker);
            var response = handler.Update(123, new OrderRepresentation());
            response.StatusCode.Should().Be.EqualTo(HttpStatusCode.NotFound);
        }

        [Test]
        public void WhenOrderExist_ThenUpdateLocation()
        {
            var order = new Order {Id = 123, Location = Location.InShop};
            var handler = new OrderResourceHandler(new RepositoryStub<Order>(order), resourceLinker);
            handler.Update(123, new OrderRepresentation {Location = Location.TakeAway});
            order.Location.Should().Be.EqualTo(Location.TakeAway);
        }
    }
}