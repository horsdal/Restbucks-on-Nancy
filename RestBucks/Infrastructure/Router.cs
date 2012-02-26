using System;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Web.Routing;
using Microsoft.ApplicationServer.Http.Activation;
using Microsoft.ApplicationServer.Http.Description;

namespace RestBucks.Infrastructure
{
    public static class RouteTableConfigurator
    {
        public static void Configure(IHttpHostConfigurationBuilder builder)
        {
            var routeInfo = typeof (RouteTableConfigurator)
                .Assembly.GetTypes()
                .Where(t => Attribute.IsDefined(t, typeof (ServiceContractAttribute))
                            && Attribute.IsDefined(t, typeof (WithUriPrefix)))
                .Select(t => new{ ServiceType = t, UriPrefix = GetPrefix(t)});

            var routes = routeInfo
                .Select(rinfo => new ServiceRoute(rinfo.UriPrefix, 
                                                  new HttpConfigurableServiceHostFactory
                                                        {
                                                            Builder = builder
                                                        }, 
                                                  rinfo.ServiceType));


            foreach (var route in routes)
            {
                RouteTable.Routes.Add(route);
            }
        }

        private static string GetPrefix(Type t)
        {
            return t.GetCustomAttributes(typeof(WithUriPrefix), true)
                    .OfType<WithUriPrefix>()
                    .First().Uri;
        }
    }
}