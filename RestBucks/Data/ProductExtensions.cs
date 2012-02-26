using System.Linq;
using RestBucks.Domain;

namespace RestBucks.Data
{
    public static class ProductExtensions
    {
        public static Product GetByName(this IRepository<Product> products, string name)
        {
            return products.Retrieve(p => p.Name.ToLower() == name.ToLower())
                                               .FirstOrDefault();
        }
    }
}