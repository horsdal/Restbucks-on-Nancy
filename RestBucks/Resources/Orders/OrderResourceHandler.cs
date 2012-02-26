using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.ServiceModel;
using System.ServiceModel.Web;
using Microsoft.ApplicationServer.Http;
using RestBucks.Data;
using RestBucks.Domain;
using RestBucks.Infrastructure;
using RestBucks.Infrastructure.Linking;
using RestBucks.Resources.Orders.Representations;

namespace RestBucks.Resources.Orders
{
    [ServiceContract, WithUriPrefix("order")]
    public class OrderResourceHandler
    {
        private readonly IRepository<Order> orderRepository;
        private readonly IResourceLinker linker;

        public OrderResourceHandler(IRepository<Order> orderRepository, IResourceLinker linker)
        {
            this.orderRepository = orderRepository;
            this.linker = linker;
        }

        [WebGet(UriTemplate = "{orderId}")]
        public HttpResponseMessage Get(int orderId, HttpRequestMessage request)
        {
            var order = orderRepository.GetById(orderId);
            if (order == null) return Responses.NotFound();
            
            if(order.Status == OrderStatus.Canceled)
            {
                return Responses.MovedTo(linker.GetUri<TrashHandler>(rh => rh.GetCanceled(0), new {orderId}));
            }

            if (request.IsNotModified(order)) return Responses.NotModified(maxAge: TimeSpan.FromSeconds(10));

            var response = Responses.WithContent(OrderRepresentationMapper.Map(order))
                                    .AddCacheHeaders(order);
            
            return response;
        }

        [WebInvoke(UriTemplate = "{orderId}", Method = "POST")]
        public HttpResponseMessage Update(int orderId, OrderRepresentation orderRepresentation)
        {
            var order = orderRepository.GetById(orderId);
            if (order == null) return Responses.NotFound();
            order.Location = orderRepresentation.Location;
            return Responses.NoContent();
        }

        [WebInvoke(UriTemplate = "{orderId}", Method = "DELETE")]
        public HttpResponseMessage Cancel(int orderId)
        {
            var order = orderRepository.GetById(orderId);
            if(order == null) return Responses.NotFound();
            order.Cancel("canceled from the rest interface");
            return Responses.NoContent();
        }

        [WebInvoke(UriTemplate = "{orderId}/payment", Method = "POST")]
        public HttpResponseMessage Pay(int orderId, PaymentRepresentation paymentArgs)
        {
            var order = orderRepository.GetById(orderId);
            if (order == null) return Responses.NotFound();
            order.Pay(paymentArgs.CardNumber, paymentArgs.CardOwner);
            return Responses.Ok();
        }

        [WebGet(UriTemplate = "{orderId}/receipt")]
        public HttpResponseMessage Receipt(int orderId)
        {
            //return Get(orderId);
            throw new NotImplementedException();
        }
    }
}