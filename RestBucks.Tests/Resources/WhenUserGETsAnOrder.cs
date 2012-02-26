using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
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
    public class WhenUserGETsAnOrder
    {
        private IResourceLinker resourceLinker = new ResourceLinker();

        [Test]
        public void WhenOrderHasNotChanged_ThenReturn304()
        {
            var handler = new OrderResourceHandler(new RepositoryStub<Order>(new Order{Version = 1, Id=123}), 
                                                    resourceLinker);

            var request = new HttpRequestMessage{Headers = {IfNoneMatch = {new EntityTagHeaderValue("\"1\"")}}};
            var response = handler.Get(123, request);
            response.StatusCode.Should().Be.EqualTo(HttpStatusCode.NotModified);
        }

        [Test]
        public void WhenOrderHasChanged_ThenReturn200()
        {
            var handler = new OrderResourceHandler(new RepositoryStub<Order>(new Order { Version = 2, Id = 123 }),
                                                    resourceLinker);

            var request = new HttpRequestMessage { Headers = { IfNoneMatch = { new EntityTagHeaderValue("\"1\"") } } };
            var response = handler.Get(123, request);
            response.StatusCode.Should().Be.EqualTo(HttpStatusCode.OK);
        }

        [Test]
        public void WhenOrderDoesNotExist_ThenReturn404()
        {
            var emptyRepository = new RepositoryStub<Order>();
            var handler = new OrderResourceHandler(emptyRepository, resourceLinker);
            var response = handler.Get(123, new HttpRequestMessage());

            response.StatusCode.Should().Be.EqualTo(HttpStatusCode.NotFound);
        }

        [Test]
        public void WhenOrderExists_ThenReturn404()
        {
            var emptyRepository = new RepositoryStub<Order>(new Order
                                                                {
                                                                    Id = 123,
                                                                    Location = Location.TakeAway
                                                                });

            var handler = new OrderResourceHandler(emptyRepository, resourceLinker);
            var response = (HttpResponseMessage<OrderRepresentation>)handler.Get(123, new HttpRequestMessage());

            response.Content.ReadAs()
                .Location.Should().Be.EqualTo(Location.TakeAway);
        }
    }
}