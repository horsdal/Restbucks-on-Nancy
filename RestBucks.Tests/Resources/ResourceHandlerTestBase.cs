namespace RestBucks.Tests.Resources
{
  using Infrastructure.Linking;

  using Nancy.Testing;
  using Nancy.TinyIoc;
  using Orders.Domain;
  using Products.Domain;
  using RestBucks.Data;
  using RestBucks.Infrastructure.Domain;
  using Util;

  public class ResourceHandlerTestBase
  {
    protected readonly Product latte = new Product
                                     {
                                       Version = 1,
                                       Name = "latte",
                                       Price = 2.5m,
                                       Customizations =
                                         {
                                           new Customization
                                           {
                                             Name = "size",
                                             PossibleValues = {"small", "medium"}
                                           }
                                         }
                                     };

    public TinyIoCContainer Container { get; private set; }

    public Browser CreateAppProxy(IRepository<Order> orderRepository = null, IRepository<Product> productRepository = null)
    {
      var defaultProductRepository = new RepositoryStub<Product>(
        latte, new Product {Version = 1, Name = "Other", Price = 3.6m});

      var bootstrapper = new ConfigurableBootstrapper
        (with =>
        {
          with.Dependency(productRepository ?? defaultProductRepository);
          with.Dependency(orderRepository ?? new RepositoryStub<Order>());
          with.AllDiscoveredModules();
        });
      var app = new Browser(bootstrapper);

      Container = (TinyIoCContainer)
        typeof(ConfigurableBootstrapper)
            .BaseType
            .GetProperty("ApplicationContainer", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .GetValue(bootstrapper, null);

      return app;
    }
  }
}