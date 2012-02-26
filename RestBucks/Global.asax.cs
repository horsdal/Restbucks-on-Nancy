using System;
using System.Web;
using Castle.Facilities.TypedFactory;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;
using Castle.Windsor;
using Castle.Windsor.Installer;
using Microsoft.ApplicationServer.Http.Description;
using RestBucks.Bots;
using RestBucks.Infrastructure;
using RestBucks.Infrastructure.WebApi;

namespace RestBucks
{
    public class Global : HttpApplication
    {
        private static readonly IWindsorContainer Container = new WindsorContainer();

        protected void Application_Start(object sender, EventArgs e)
        {
            Container.AddFacility<TypedFactoryFacility>();
            Container.Kernel.Resolver.AddSubResolver(new CollectionResolver(Container.Kernel, true));
            Container.Install(FromAssembly.This());

            var httpOperationHandlerFactory = Container.Resolve<HttpOperationHandlerFactory>();
            var config = HttpHostConfiguration.Create()
                .SetResourceFactory(new WindsorResourceFactory(Container))
                .SetMessageHandlerFactory(new DelegatingChannelFactory(Container))
                .SetOperationHandlerFactory(httpOperationHandlerFactory);

            RouteTableConfigurator.Configure(config);
            //Initialize Barista
            Container.Resolve<Barista>();
        }


        protected void Application_Error(object sender, EventArgs e)
        {
        }

        protected void Application_End(object sender, EventArgs e)
        {
            Container.Dispose();
        }
    }
}