namespace RestBucks.Data
{
  using System.Linq;
  using Products.Domain;

  public static class ProductExtensions
  {
    public static Product GetByName(this IRepository<Product> products, string name)
    {
      return products.Retrieve(p => p.Name.ToLower() == name.ToLower())
        .FirstOrDefault();
    }
  }
}