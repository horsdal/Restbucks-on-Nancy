namespace RestBucks.Infrastructure
{
  using System.IO;
  using System.Yaml.Serialization;
  using Nancy.Responses.Negotiation;
  using System;
  using System.Collections.Generic;
  using Nancy;
  using Nancy.Responses;

  public class YamlWrapper : ISerializer
  {
    private readonly YamlSerializer serializer;

    public YamlWrapper(YamlSerializer serializer)
    {
      this.serializer = serializer;
    }

    public bool CanSerialize(string contentType)
    {
      return contentType.EndsWith("yaml", StringComparison.InvariantCultureIgnoreCase);
    }

    public void Serialize<TModel>(string contentType, TModel model, Stream outputStream)
    {
      serializer.Serialize(outputStream, model);
    }

    public IEnumerable<string> Extensions { get { yield return "yaml"; } }
  }


  public class YamlResponseProcessor : IResponseProcessor
  {
    private readonly ISerializer yamlSerializer;

    public YamlResponseProcessor(YamlWrapper serializer)
    {
      yamlSerializer = serializer;
    }

    public ProcessorMatch CanProcess(MediaRange requestedMediaRange, dynamic model, NancyContext context)
    {
      if (IsWildCardOrApplicationType(requestedMediaRange.Type) && IsWildCardOrYamlSubType(requestedMediaRange.Subtype))
        return new ProcessorMatch {RequestedContentTypeResult = MatchResult.ExactMatch, ModelResult = MatchResult.DontCare};
      else
        return new ProcessorMatch {ModelResult = MatchResult.NoMatch, RequestedContentTypeResult = MatchResult.NoMatch};
    }

    private static bool IsWildCardOrYamlSubType(MediaType subtype)
    {
      return (subtype.IsWildcard || subtype.ToString().ToLower().Equals("yaml") || subtype.ToString().ToLower().Contains("+yaml"));
    }

    private static bool IsWildCardOrApplicationType(MediaType superType)
    {
      return (superType.IsWildcard || superType.ToString().ToLower().Equals("application"));
    }

    public Response Process(MediaRange requestedMediaRange, dynamic model, NancyContext context)
    {
      var xmlResponseType = typeof (XmlResponse<>).MakeGenericType(model.GetType());
      return Activator.CreateInstance(xmlResponseType, model, "application/vnd.restbucks+yaml", yamlSerializer);
    }

    public IEnumerable<Tuple<string, MediaRange>> ExtensionMappings
    {
      get
      {
        yield return new Tuple<string, MediaRange>("yaml", "application/vnd.restbucks+yaml");
      }
    }
  }
}