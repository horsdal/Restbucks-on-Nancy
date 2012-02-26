using System;
using System.Configuration;
using System.ServiceModel;
using Enumerable = System.Linq.Enumerable;

namespace RestBucks.Infrastructure
{
    public static class BaseAddress
    {
        public static string Current
        {
            get
            {
                return ConfigurationManager.AppSettings["baseUri"] ?? GetFromWcf();
            }
        }

        private static string GetFromWcf()
        {
            var fullUrl = Enumerable.Single(OperationContext.Current.Host.Description.Endpoints).Address.Uri;
            return fullUrl.ToString().Substring(0, fullUrl.ToString().Length - fullUrl.LocalPath.Length);
        }
    }
}