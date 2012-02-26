using System;
using System.Net.Http;
using System.ServiceModel;
using Castle.Windsor;
using Microsoft.ApplicationServer.Http.Description;
using RestBucks.Resources.Orders;
using RestBucks.Resources.Orders.Representations;

namespace RestBucks.Infrastructure.WebApi
{
    public class WindsorResourceFactory : IResourceFactory
    {
        private readonly IWindsorContainer container;

        public WindsorResourceFactory(IWindsorContainer container)
        {
            this.container = container;
        }

        public object GetInstance(Type serviceType, InstanceContext instanceContext, HttpRequestMessage request)
        {
            return container.Resolve(serviceType);
        }

        public void ReleaseInstance(InstanceContext instanceContext, object service)
        {
            container.Release(service);
        }
    }
}