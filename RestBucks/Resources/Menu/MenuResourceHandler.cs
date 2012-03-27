namespace RestBucks.Resources.Products
{
  using System;
  using System.Collections.Generic;
  using System.Linq;

  using Data;
  using Domain;

  using Infrastructure;

  using Nancy;

  public class MenuResourceHandler : NancyModule
  {
    private readonly IRepository<Product> productRepository;

    public MenuResourceHandler(IRepository<Product> productRepository) : base("menu")
    {
      this.productRepository = productRepository;

      Get["/"] = _ => GetHandler();
    }

    private Response GetHandler()
    {
      var products = productRepository.RetrieveAll().OrderBy(p => p.Name).ToList();
      var menuRepresentation = CreateMenuRepresentation(products);

      return
        Response
        .WithContent(Request.Headers.Accept, menuRepresentation)
        .WithCacheHeaders(products.First(), TimeSpan.FromHours(6));
    }

    private static MenuRepresentation CreateMenuRepresentation(List<Product> products)
    {
      var itemRepresentations = products
        .Select(p => new ItemRepresentation {Name = p.Name, Price = p.Price})
        .ToArray();

      return new MenuRepresentation {Items = itemRepresentations};
    }
  }
}