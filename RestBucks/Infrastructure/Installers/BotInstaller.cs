namespace RestBucks.Infrastructure.Installers
{
  using Castle.MicroKernel.Registration;
  using Castle.MicroKernel.SubSystems.Configuration;
  using Castle.Windsor;

  using Bots;

  public class BotInstaller : IWindsorInstaller
  {
    public void Install(IWindsorContainer container, IConfigurationStore store)
    {
      container.Register(Component.For<Barista>());
    }
  }
}