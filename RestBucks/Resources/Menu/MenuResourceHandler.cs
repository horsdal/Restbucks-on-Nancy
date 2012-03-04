namespace RestBucks.Resources.Products
{
  using System.Linq;

  using Data;
  using Domain;

  using Nancy;

  public class MenuResourceHandler : NancyModule
  {
    private readonly IRepository<Product> productRepository;

    public MenuResourceHandler(IRepository<Product> productRepository) : base("menu")
    {
      this.productRepository = productRepository;

      Get["/"] = _ => Response.AsXml(GetHandler());
    }

    private MenuRepresentation GetHandler()
    {
      var products = productRepository.RetrieveAll().OrderBy(p => p.Name)
        .ToList()
        .Select(p => new ItemRepresentation {Name = p.Name, Price = p.Price}).ToArray();
      return new MenuRepresentation {Items = products};
    }
  }
}