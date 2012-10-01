﻿namespace RestBucks.Infrastructure.Linking
{
  using System;
  using System.Collections.Generic;
  using System.ComponentModel;
  using System.Linq;

  public class ResourceLinker
  {
    private readonly string baseUri;

    public ResourceLinker(string httpBaseuri)
    {
      baseUri = httpBaseuri;
    }

    public string BuildUriString(string prefix, string template, dynamic parameters)
    {
      var newBaseUri = new Uri(baseUri.TrimEnd('/') + prefix);
      var uriTemplate = new UriTemplate(template, true);

      return uriTemplate.BindByName(newBaseUri, ToDictionary(parameters ?? new {})).ToString();
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