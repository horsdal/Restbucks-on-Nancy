using System;
using System.Net.Http;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using RestBucks.Infrastructure.SessionManagement;
using RestBucks.Infrastructure.WebApi;

namespace RestBucks.Infrastructure.Installers
{
    public class ChannelInstaller : IWindsorInstaller
    {
        public const string MostInnerChannelKey = "mostInnerHttpChannel";
        public const string TopLevelChannelKey = "topLevelHttpChannel";

        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Component.For<HttpMessageChannel>()
                                        .ImplementedBy<NHibernateMessageChannel>()
                                        .ServiceOverrides(ServiceOverride.ForKey("innerChannel").Eq(MostInnerChannelKey))
                                        .Named(TopLevelChannelKey));
        }
    }
}