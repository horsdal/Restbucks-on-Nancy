namespace RestBucks
{
  using System.ComponentModel;

  using Bots;

  using Castle.Facilities.TypedFactory;
  using Castle.MicroKernel.Resolvers.SpecializedResolvers;
  using Castle.Windsor.Installer;

  using Nancy.Bootstrappers.Windsor;

  public class Bootstrapper : WindsorNancyBootstrapper
  {
    protected override void ConfigureApplicationContainer(Castle.Windsor.IWindsorContainer existingContainer)
    {
      base.ConfigureApplicationContainer(existingContainer);

      existingContainer.AddFacility<TypedFactoryFacility>();
      existingContainer.Kernel.Resolver.AddSubResolver(new CollectionResolver(existingContainer.Kernel, true));
      existingContainer.Install(FromAssembly.This());
    }

    protected override void ApplicationStartup(Castle.Windsor.IWindsorContainer container, Nancy.Bootstrapper.IPipelines pipelines)
    {
      base.ApplicationStartup(container, pipelines);

      container.Resolve<Barista>();
    }
  }
}