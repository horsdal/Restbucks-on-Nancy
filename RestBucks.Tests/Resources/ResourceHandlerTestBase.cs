namespace RestBucks.Tests.Resources
{
  using Infrastructure.Linking;

  using Nancy.Testing;

  using RestBucks.Data;
  using RestBucks.Domain;

  using Util;

  public abstract class ResourceHandlerTestBase
  {
    protected readonly Product latte = new Product
                                     {
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
        latte, new Product {Name = "Other", Price = 3.6m});

      return new Browser(
        new ConfigurableBootstrapper
          (with =>
           {
             with.Dependency<IRepository<Product>>(productRepository ?? defaultProductRepository);
             with.Dependency<IRepository<Order>>(orderRepository ?? new RepositoryStub<Order>());
             with.Dependency<IResourceLinker>(new ResourceLinker("http://bougs"));
           }
          ));
    }
  }
}