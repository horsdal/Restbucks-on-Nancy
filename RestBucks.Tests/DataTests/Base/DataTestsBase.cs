using log4net.Config;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using NUnit.Framework;
using RestBucks.Infrastructure.Installers;

namespace RestBucks.Tests.DataTests.Base
{
    public class DataTestsBase
    {
        protected ISessionFactory sessionFactory;
        private Configuration configuration;

        [TestFixtureSetUp]
        public void FixtureSetUp()
        {
            XmlConfigurator.Configure();
            configuration = NHibernateInstaller.CreateConfiguration("Restbucks_Tests");
            new SchemaExport(configuration).Execute(true, true , false);
            sessionFactory = configuration.BuildSessionFactory();
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            new SchemaExport(configuration).Execute(true,true, true);
        }
    }
}