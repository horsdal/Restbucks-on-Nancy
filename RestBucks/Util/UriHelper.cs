using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace RestBucks.Util
{
    public class UriHelper
    {
        public static string ExcuteUriTemplate(string baseAddress, string templateUri, object args = null)
        {
            var newBaseUri = new Uri(baseAddress, UriKind.Absolute);
            var uriTemplate = new UriTemplate(templateUri, true);
            return uriTemplate.BindByName(newBaseUri, ToDictionary(args ?? new { })).ToString();
        }

        public static string Combine(string baseAddress, string nestedUri)
        {
            var combined = new Uri(new Uri(baseAddress,UriKind.Absolute), nestedUri);
            return combined.ToString();
        }

        private static IDictionary<string, string> ToDictionary(object anonymousInstance)
        {
            var dictionary = anonymousInstance as IDictionary<string, string>;
            if (dictionary != null) return dictionary;

            return TypeDescriptor.GetProperties(anonymousInstance)
                .OfType<PropertyDescriptor>()
                .ToDictionary(p => p.Name, p => p.GetValue(anonymousInstance).ToString());
        }
    }
}