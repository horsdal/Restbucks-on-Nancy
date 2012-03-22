namespace RestBucks.Infrastructure.Installers
{
  using Castle.MicroKernel.Registration;
  using Castle.MicroKernel.SubSystems.Configuration;
  using Castle.Windsor;

  using RestBucks.Data;
  using RestBucks.Data.Impl;

  public class RepositoryInstaller : IWindsorInstaller
  {
    public void Install(IWindsorContainer container, IConfigurationStore store)
    {
      container.Register(Component.For(typeof (IRepository<>)).ImplementedBy(typeof (Repository<>)));
    }
  }
}