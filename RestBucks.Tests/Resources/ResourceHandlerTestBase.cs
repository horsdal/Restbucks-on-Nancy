namespace RestBucks.Tests.Resources
{
  using Infrastructure.Linking;

  using Nancy.Testing;
  using Orders.Domain;
  using Products.Domain;
  using RestBucks.Data;
  using RestBucks.Infrastructure.Domain;
  using Util;

  public abstract class ResourceHandlerTestBase
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

    public Browser CreateAppProxy(IRepository<Order> orderRepository = null, IRepository<Product> productRepository = null)
    {
      var defaultProductRepository = new RepositoryStub<Product>(
        latte, new Product {Version = 1, Name = "Other", Price = 3.6m});

      return new Browser(
        new ConfigurableBootstrapper
          (with =>
           {
             with.Dependency<IRepository<Product>>(productRepository ?? defaultProductRepository);
             with.Dependency<IRepository<Order>>(orderRepository ?? new RepositoryStub<Order>());
           }
          ));
    }
  }
}