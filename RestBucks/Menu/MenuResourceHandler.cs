namespace RestBucks.Menu
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using Data;
  using Infrastructure;
  using Nancy;
  using Products.Domain;

  public class MenuResourceHandler : NancyModule
  {
    private readonly IRepository<Product> productRepository;
    private static readonly TimeSpan menuMaxAge = TimeSpan.FromHours(6);

    public MenuResourceHandler(IRepository<Product> productRepository) : base("menu")
    {
      this.productRepository = productRepository;

      Get["/"] = _ => GetHandler();
    }

    private Object GetHandler()
    {
      var products = productRepository.RetrieveAll().OrderBy(p => p.Name).ToList();
      var menuRepresentation = CreateMenuRepresentation(products);

      if (Request.IsNotModified(products.First()))
        return Response.NotModified(menuMaxAge);
      else
        return
          Negotiate
          .WithModel(menuRepresentation)
          .WithCacheHeaders(products.First(), UntilMidnight());
    }

    private static MenuRepresentation CreateMenuRepresentation(List<Product> products)
    {
      var itemRepresentations = products
        .Select(p => new ItemRepresentation {Name = p.Name, Price = p.Price})
        .ToArray();

      return new MenuRepresentation {Items = itemRepresentations};
    }

    private static TimeSpan UntilMidnight()
    {
      return DateTime.UtcNow.Date.AddDays(1).Subtract(DateTime.UtcNow);
    }
  }
}