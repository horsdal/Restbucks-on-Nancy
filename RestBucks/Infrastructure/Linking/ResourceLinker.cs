using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.ServiceModel.Web;

namespace RestBucks.Infrastructure.Linking
{
    public class ResourceLinker : IResourceLinker
    {
        private readonly Uri baseUri;

        public ResourceLinker()
        {
            baseUri = new Uri(BaseAddress.Current, UriKind.Absolute);
        }
        
        public string GetUri<T>(Expression<Action<T>> method, object uriArgs = null)
        {
            string prefix = GetServicePrefix<T>();

            var methodInfo = ((MethodCallExpression)method.Body).Method;
            var methodTemplate = GetUriTemplateForMethod(methodInfo);
        
            var newBaseUri = new Uri(baseUri, prefix);
            var uriTemplate = new UriTemplate(methodTemplate, true);

            return uriTemplate.BindByName(newBaseUri, ToDictionary(uriArgs ?? new{})).ToString();
        }

        
        public static IDictionary<string, string> ToDictionary(object anonymousInstance)
        {
            var dictionary = anonymousInstance as IDictionary<string, string>;
            if (dictionary != null) return dictionary;

            return TypeDescriptor.GetProperties(anonymousInstance)
                .OfType<PropertyDescriptor>()
                .ToDictionary(p => p.Name, p => p.GetValue(anonymousInstance).ToString());
        }

        private static string GetServicePrefix<T>()
        {
            var withUriPrefixAttribute = typeof (T)
                                .GetCustomAttributes(typeof (WithUriPrefix), true)
                                .OfType<WithUriPrefix>()
                                .FirstOrDefault();

            if(withUriPrefixAttribute == null )
            {
                var message = string.Format("Can't find the WithUriPrefix in {0}", typeof(T).Name);
                throw new InvalidOperationException(message);
            }
            return withUriPrefixAttribute.Uri;
        }

        private static string GetUriTemplateForMethod(MethodInfo method)
        {
            var webGet = method.GetCustomAttributes(true)
                                .OfType<WebGetAttribute>()
                                .FirstOrDefault();
            if (webGet != null) return webGet.UriTemplate ?? method.Name;

            var webInvoke = method.GetCustomAttributes(true)
                                .OfType<WebInvokeAttribute>()
                                .FirstOrDefault();
            if (webInvoke != null) return webInvoke.UriTemplate ?? method.Name;

            var message = string.Format("The method {0} is not a web method.", method.Name);
            throw new InvalidOperationException(message);
        }
    }
}