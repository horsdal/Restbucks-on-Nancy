namespace RestBucks.Resources.Products
{
  using System.Linq;
  using System.ServiceModel;
  using System.ServiceModel.Web;

  using RestBucks.Data;
  using RestBucks.Domain;
  using RestBucks.Infrastructure;

  [ServiceContract, WithUriPrefix("menu")]
  public class MenuResourceHandler
  {
    private readonly IRepository<Product> productRepository;

    public MenuResourceHandler(IRepository<Product> productRepository)
    {
      this.productRepository = productRepository;
    }

    [WebGet(UriTemplate = "")]
    public MenuRepresentation Get()
    {
      var products = productRepository.RetrieveAll().OrderBy(p => p.Name)
        .ToList()
        .Select(p => new ItemRepresentation {Name = p.Name, Price = p.Price}).ToArray();
      return new MenuRepresentation {Items = products};
    }
  }
}