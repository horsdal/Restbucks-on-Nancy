using System.Net.Http;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Microsoft.ApplicationServer.Http.Channels;
using RestBucks.Infrastructure.Installers;

namespace RestBucks.Infrastructure.WebApi
{
    public class DelegatingChannelFactory : HttpMessageHandlerFactory
    {
        private readonly IWindsorContainer container;

        public DelegatingChannelFactory(IWindsorContainer container)
        {
            this.container = container;
        }

        protected override HttpMessageChannel OnCreate(HttpMessageChannel innerChannel)
        {

            if (!container.Kernel.HasComponent(ChannelInstaller.MostInnerChannelKey))
            {
                container.Register(Component.For<HttpMessageChannel>()
                                        .Instance(base.OnCreate(innerChannel))
                                        .Named(ChannelInstaller.MostInnerChannelKey));
            }

            return container.Resolve<HttpMessageChannel>(ChannelInstaller.TopLevelChannelKey);
        }
    }
}