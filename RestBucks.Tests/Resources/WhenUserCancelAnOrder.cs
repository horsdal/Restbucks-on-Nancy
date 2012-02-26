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
    public class WhenUserCancelAnOrder
    {
        private IResourceLinker resourceLinker = new ResourceLinker();

        [Test]
        public void WhenOrderDoesNotExist_ThenReturn404()
        {
            var handler = new OrderResourceHandler(new RepositoryStub<Order>(), resourceLinker);
            var response = handler.Cancel(123);
            response.StatusCode.Should().Be.EqualTo(HttpStatusCode.NotFound);
        }

        [Test]
        public void WhenOrderExist_ThenCancel()
        {
            var order = new Order { Id = 123 };
            var handler = new OrderResourceHandler(new RepositoryStub<Order>(order), resourceLinker);
            handler.Cancel(123);
            order.Status.Should().Be.EqualTo(OrderStatus.Canceled);
        }

        [Test]
        public void WhenOrderExist_ThenCancelAndReturn204()
        {
            var order = new Order { Id = 123 };
            var handler = new OrderResourceHandler(new RepositoryStub<Order>(order), resourceLinker);
            var response = handler.Cancel(123);
            response.StatusCode.Should().Be.EqualTo(HttpStatusCode.NoContent);
        }

        [Test]
        public void ACallToGet_ShouldReturnMovedPermanentlyAndNewLocation()
        {
            var order = new Order { Id = 123 };
            var handler = new OrderResourceHandler(new RepositoryStub<Order>(order), resourceLinker);
            handler.Cancel(123);

            
            var responseToGet = handler.Get(123, null);

            var expected = "http://restbuckson.net/trash/order/123";
            responseToGet.Satisfy(r => r.StatusCode == HttpStatusCode.MovedPermanently
                                    && r.Headers.Location.ToString() == expected);
        }

        [Test]
        public void ACallToGetCanceled_ShouldReturnTheOrder()
        {
            var order = new Order { Id = 123 };
            var orderRepository = new RepositoryStub<Order>(order);
            var handler = new OrderResourceHandler(orderRepository, resourceLinker);
            handler.Cancel(123);

            //var expected = resourceLinker.GetUri<OrderResourceHandler>(rh => rh.GetCanceled(0), new { orderId = 123 });
            var cancelHandler = new TrashHandler(orderRepository);
            var responseToGet = (HttpResponseMessage<OrderRepresentation>)cancelHandler.GetCanceled(123);

            responseToGet.Satisfy(r => r.StatusCode == HttpStatusCode.OK
                                    && r.Content.ReadAs() != null);
        }
    }
}