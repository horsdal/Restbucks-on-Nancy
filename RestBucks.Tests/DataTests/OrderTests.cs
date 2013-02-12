namespace RestBucks.Tests.Data
{
  using System;
  using System.Linq;

  using NUnit.Framework;
  using Orders.Domain;
  using Products.Domain;
  using RestBucks.Data.Impl;
  using DataTests.Base;
  using SharpTestsEx;

  [TestFixture]
  public class OrderTests : DataTestsBase
  {
    [Test]
    public void CanStoreAOrder()
    {
      long id;
      using (var session = sessionFactory.OpenSession())
      using (var tx = session.BeginTransaction())
      {
        var productRepository = new Repository<Product>(session);
        var product = new Product {Name = "Latte", Price = 10.4m};
        productRepository.MakePersistent(product);

        var orderRepository = new Repository<Order>(session);
        var order = new Order
                    {
                      Date = new DateTime(2011, 1, 1),
                      Location = Location.InShop
                    };

        order.AddItem(new OrderItem
                      {
                        Product = product,
                        UnitPrice = 10.4m,
                        Preferences =
                          {
                            {"Milk", "skim"},
                            {"Size", "small"}
                          }
                      });

        order.AddItem(new OrderItem
                      {
                        Product = product,
                        UnitPrice = 10.4m,
                        Preferences = {{"Shots", "single"}}
                      });

        orderRepository.MakePersistent(order);
        id = order.Id;
        tx.Commit();
      }

      using (var context = sessionFactory.OpenSession())
      {
        var repository = new Repository<Order>(context);
        var order = repository.GetById(id);
        order.Satisfy(o => o.Location == Location.InShop
                           && o.Items.Count() == 2
                           && o.Items.Any(i => i.Preferences.ContainsKey("Shots"))
                           && o.Items.Any(i => i.Preferences.ContainsKey("Milk") && i.Preferences.ContainsKey("Size")));
      }
    }

    [Test]
    public void CanStoreAnOrderWithPayment()
    {
      long id;
      using (var session = sessionFactory.OpenSession())
      using (var tx = session.BeginTransaction())
      {
        var productRepository = new Repository<Product>(session);
        var product = new Product {Name = "Latte", Price = 10.4m};
        productRepository.MakePersistent(product);

        var orderRepository = new Repository<Order>(session);
        var order = new Order
                    {
                      Date = new DateTime(2011, 1, 1),
                      Location = Location.InShop,
                    };
        order.AddItem(new OrderItem
                      {
                        Product = product,
                        UnitPrice = 10.4m,
                        Preferences =
                          {
                            {"Milk", "skim"},
                            {"Size", "small"}
                          }
                      });
        orderRepository.MakePersistent(order);
        order.Pay("1234", "jose");
        id = order.Id;
        tx.Commit();
      }

      using (var context = sessionFactory.OpenSession())
      {
        var repository = new Repository<Order>(context);
        var order = repository.GetById(id);
        order.Satisfy(o => o.Location == Location.InShop
                           && o.Items.Count() == 1
                           && o.Payment != null);
      }
    }

    [Test]
    public void CanChangeStatus()
    {
      long id;
      using (var session = sessionFactory.OpenSession())
      using (var tx = session.BeginTransaction())
      {
        var productRepository = new Repository<Product>(session);
        var product = new Product {Name = "Latte", Price = 10.4m};
        productRepository.MakePersistent(product);

        var orderRepository = new Repository<Order>(session);
        var order = new Order
                    {
                      Date = new DateTime(2011, 1, 1),
                      Location = Location.InShop,
                    };
        order.AddItem(new OrderItem
                      {
                        Product = product,
                        UnitPrice = 10.4m,
                        Preferences =
                          {
                            {"Milk", "skim"},
                            {"Size", "small"}
                          }
                      });
        orderRepository.MakePersistent(order);
        order.Cancel("cascasas");
        id = order.Id;
        tx.Commit();
      }
      using (var session = sessionFactory.OpenSession())
      using (session.BeginTransaction())
      {
        session.Get<Order>(id).Status.Should().Be.EqualTo(OrderStatus.Canceled);
      }
    }

    [Test]
    public void VersionNumberGrowOnEachUpdate()
    {
      long id;
      int version;
      using (var session = sessionFactory.OpenSession())
      using (var tx = session.BeginTransaction())
      {
        var productRepository = new Repository<Product>(session);
        var product = new Product {Name = "Latte", Price = 10.4m};
        productRepository.MakePersistent(product);

        var orderRepository = new Repository<Order>(session);
        var order = new Order
                    {
                      Date = new DateTime(2011, 1, 1),
                      Location = Location.InShop,
                    };
        order.AddItem(new OrderItem
                      {
                        Product = product,
                        UnitPrice = 10.4m,
                        Preferences =
                          {
                            {"Milk", "skim"},
                            {"Size", "small"}
                          }
                      });
        orderRepository.MakePersistent(order);
        order.Pay("1234", "jose");
        id = order.Id;

        tx.Commit();
        version = order.Version;
      }
      using (var session = sessionFactory.OpenSession())
      using (var tx = session.BeginTransaction())
      {
        var order = session.Get<Order>(id);
        order.Location = Location.TakeAway;
        tx.Commit();

        order.Version.Should().Be.GreaterThan(version);
      }
    }
  }
}