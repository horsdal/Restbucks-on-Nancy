using System.Linq;
using System.Net.Http;
using System.ServiceModel;
using System.ServiceModel.Web;
using Microsoft.ApplicationServer.Http;
using RestBucks.Data;
using RestBucks.Domain;
using RestBucks.Infrastructure;
using RestBucks.Resources.Orders.Representations;

namespace RestBucks.Resources.Orders
{
    //TODO: I will wait WebAPI to add better support for nested resources.
    [ServiceContract, WithUriPrefix("trash")]
    public class TrashHandler
    {
        private readonly IRepository<Order> orderRepository;

        public TrashHandler(IRepository<Order> orderRepository)
        {
            this.orderRepository = orderRepository;
        }
        
        [WebGet(UriTemplate = "/order/{orderId}")]
        public HttpResponseMessage GetCanceled(int orderId)
        {

            

            var order = orderRepository.Retrieve(o => o.Id == orderId && o.Status == OrderStatus.Canceled)
                                       .FirstOrDefault();
            return order == null ? Responses.NotFound()
                : new HttpResponseMessage<OrderRepresentation>(OrderRepresentationMapper.Map(order));
        }
    }
}