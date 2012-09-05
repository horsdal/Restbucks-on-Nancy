namespace RestBucks.Tests
{
  using System.Linq;

  using NHibernate;
  using NHibernate.Tool.hbm2ddl;

  using NUnit.Framework;

  using RestBucks.Data.Impl;
  using RestBucks.Domain;

  using Infrastructure.Installers;

  [TestFixture]
  [Ignore("This not really a test, but a utility for creating and populating the RestBucks DB")]
  public class DataInitializer
  {
    private ISessionFactory sessionFactory;

    [TestFixtureSetUp]
    public void SetUp()
    {
      var configuration = NHibernateInstaller.CreateConfiguration();
      new SchemaExport(configuration).Execute(true, true, false);
      sessionFactory = configuration.BuildSessionFactory();
    }

    [Test]
    public void InitializeData()
    {
      using (var session = sessionFactory.OpenSession())
      using (var tx = session.BeginTransaction())
      {
        var milk = new Customization
                   {
                     Name = "Milk",
                     PossibleValues = {"skim", "semi", "whole"}
                   };

        var size = new Customization
                   {
                     Name = "Size",
                     PossibleValues = {"small", "medium", "large"}
                   };

        var shots = new Customization
                    {
                      Name = "Shots",
                      PossibleValues = {"single", "double", "triple"}
                    };

        var whippedCream = new Customization
                           {
                             Name = "Whipped Cream",
                             PossibleValues = {"yes", "no"}
                           };

        var kindOfCookie = new Customization
                           {
                             Name = "Kind",
                             PossibleValues = {"chocolate chip", "ginger"}
                           };


        var customizationRepository = new Repository<Customization>(session);
        customizationRepository.MakePersistent(milk, size, shots, whippedCream, kindOfCookie);
        var productRepository = new Repository<Product>(session);

        var coffees = new[] {"Latte", "Capuccino", "Espresso", "Tea"}
          .Select(
            coffeName =>
            new Product {Name = coffeName, Price = (decimal) coffeName.First()/10, Customizations = {milk, size, shots}})
          .ToArray();

        productRepository.MakePersistent(coffees);

        productRepository.MakePersistent(new Product
                                         {
                                           Name = "Hot Chocolate",
                                           Price = 10.5m,
                                           Customizations = {milk, size, whippedCream}
                                         });
        productRepository.MakePersistent(new Product {Name = "Cookie", Price = 1, Customizations = {kindOfCookie}});
        tx.Commit();
      }
    }
  }
}