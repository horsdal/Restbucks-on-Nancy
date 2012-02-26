using Castle.Facilities.TypedFactory;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;
using Castle.Windsor;
using Castle.Windsor.Installer;
using Microsoft.ApplicationServer.Http.Description;
using Microsoft.ApplicationServer.Http.Dispatcher;
using RestBucks.Bots;
using RestBucks.Infrastructure;
using RestBucks.Infrastructure.WebApi;

[assembly: WebActivator.PostApplicationStartMethod(
    typeof(RestBucks.App_Start.WebApiInitializer), "Initialize")]

[assembly: WebActivator.ApplicationShutdownMethod(
    typeof(RestBucks.App_Start.WebApiInitializer), "Finalize")]

namespace RestBucks.App_Start
{   
    public class WebApiInitializer
    {
        private static readonly IWindsorContainer Container = new WindsorContainer();

        public static void Initialize()
        {
            
            Container.AddFacility<TypedFactoryFacility>();
            Container.Kernel.Resolver.AddSubResolver(new CollectionResolver(Container.Kernel, true));
            Container.Install(FromAssembly.This());

            var config = HttpHostConfiguration.Create()
                            .SetErrorHandler(Container.Resolve<HttpErrorHandler>())
                            .SetResourceFactory(new WindsorResourceFactory(Container))
                            .SetOperationHandlerFactory(Container.Resolve<HttpOperationHandlerFactory>());

            RouteTableConfigurator.Configure(config);

            //Initialize Barista
            Container.Resolve<Barista>();

        }

#pragma warning disable 465
        public static void Finalize()
#pragma warning restore 465
        {
            Container.Dispose();
        }
    }
}