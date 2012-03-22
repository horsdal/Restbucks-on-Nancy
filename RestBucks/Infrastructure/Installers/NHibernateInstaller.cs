namespace RestBucks.Infrastructure.Installers
{
  using System.Collections.Generic;

  using Castle.Facilities.TypedFactory;
  using Castle.MicroKernel;
  using Castle.MicroKernel.Registration;
  using Castle.MicroKernel.SubSystems.Configuration;
  using Castle.Windsor;

  using Data;

  using NHibernate;
  using NHibernate.Cfg;
  using NHibernate.Context;
  using NHibernate.Dialect;

  using RestBucks.Data;

  public class NHibernateInstaller : IWindsorInstaller
  {
    public void Install(IWindsorContainer container, IConfigurationStore store)
    {
      container.Register(Component.For<ISessionFactory>()
                           .UsingFactoryMethod(BuildSessionFactory));

      container.Register(Component.For<IEnumerable<ISessionFactory>>()
                           .UsingFactoryMethod((k, c) => new[] {k.Resolve<ISessionFactory>()}));
    }

    public static ISessionFactory BuildSessionFactory(IKernel kernel)
    {
      var configuration = CreateConfiguration();
      return configuration.BuildSessionFactory();
    }

    public static Configuration CreateConfiguration(string connectionStringName = "RestBucks")
    {
      var configuration = new Configuration();

      configuration.DataBaseIntegration(db =>
                                        {
                                          db.Dialect<MsSql2008Dialect>();
                                          db.ConnectionStringName = connectionStringName;
                                          db.LogFormatedSql = true;
                                          db.KeywordsAutoImport = Hbm2DDLKeyWords.AutoQuote;
                                        });
      configuration.CurrentSessionContext<CallSessionContext>();
      configuration.CollectionTypeFactory<Net4CollectionTypeFactory>();
      configuration.AddDeserializedMapping(Mapper.Generate(), "Restbucks");
      return configuration;
    }
  }
}