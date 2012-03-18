namespace RestBucks.Infrastructure.Installers
{
  using System;
  using System.ServiceModel;

  using Castle.MicroKernel.Registration;
  using Castle.MicroKernel.SubSystems.Configuration;
  using Castle.Windsor;

  using Microsoft.ApplicationServer.Http.Description;
  using Microsoft.ApplicationServer.Http.Dispatcher;

  using Linking;

  using WebApi;

  public class WebApiInstaller : IWindsorInstaller
  {
    public void Install(IWindsorContainer container, IConfigurationStore store)
    {
      container.Register(Component.For<IResourceLinker>()
                           .ImplementedBy<ResourceLinker>());

      container.Register(AllTypes.FromThisAssembly()
                           .Where(t => Attribute.IsDefined(t, typeof (ServiceContractAttribute)))
                           .WithService.Self()
                           .Configure(c => c.Interceptors(new[] {typeof (RestBucksHttpErrorHandler)})));

      container.Register(Component.For<RestBucksHttpErrorHandler>());
      //TODO: remove the ErrorHandlerInterceptor.

      container.Register(AllTypes.FromThisAssembly()
                           .BasedOn<HttpErrorHandler>()
                           .WithService.Base());

      container.Register(Component.For<HttpOperationHandlerFactory>());
    }
  }
}