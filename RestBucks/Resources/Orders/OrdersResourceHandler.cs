using System;
using System.Net.Http;
using System.ServiceModel;
using System.ServiceModel.Web;
using RestBucks.Data;
using RestBucks.Domain;
using RestBucks.Infrastructure;
using RestBucks.Infrastructure.Linking;
using RestBucks.Infrastructure.WebApi;
using RestBucks.Resources.Orders.Representations;

namespace RestBucks.Resources.Orders
{
    [ServiceContract, WithUriPrefix("orders")]
    public class OrdersResourceHandler
    {
        private readonly IRepository<Product> productRepository;
        private readonly IRepository<Order> orderRepository;
        private readonly IResourceLinker resourceLinker;

        public OrdersResourceHandler(
                IRepository<Product> productRepository,
                IRepository<Order> orderRepository,
                IResourceLinker resourceLinker)
        {
            this.productRepository = productRepository;
            this.orderRepository = orderRepository;
            this.resourceLinker = resourceLinker;
        }

        [WebInvoke(Method = "POST", UriTemplate = "")]
        public HttpResponseMessage Create(OrderRepresentation orderRepresentation)
        {
            var order = new Order
            {
                Date = DateTime.Today,
                Location = orderRepresentation.Location
            };

            foreach (var requestedItem in orderRepresentation.Items)
            {
                var product = productRepository.GetByName(requestedItem.Name);
                if (product == null)
                {
                    return Responses.BadRequest(string.Format("We don't offer {0}", requestedItem.Name));
                }
                var orderItem = new OrderItem(product,
                                            requestedItem.Quantity,
                                            product.Price,
                                            requestedItem.Preferences);
                order.AddItem(orderItem);
            }

            if (!order.IsValid())
            {
                var content = string.Join("\n", order.GetErrorMessages());
                return Responses.BadRequest("Invalid entities values", content);
            }

            orderRepository.MakePersistent(order);
            var uri = resourceLinker.GetUri<OrderResourceHandler>(
                                                        orderResource => orderResource.Get(0, null),
                                                        new { orderId = order.Id });
            return Responses.Created(uri);
        }
    }
}