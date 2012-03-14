using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.ApplicationServer.Http;
using RestBucks.Domain.BaseClass;

namespace RestBucks.Infrastructure
{
  using Nancy;

  using HttpStatusCode = System.Net.HttpStatusCode;

  public static class ResponseHelpers
  {
    public static Response BadRequest(this IResponseFormatter formatter, string reason = "", string content = "")
    {
      Response response = content;
      response.StatusCode = Nancy.HttpStatusCode.BadRequest;
      return response.WithHeader("ReasonPhrase", reason);
    }

    public static Response Created(this IResponseFormatter formatter, string location)
    {
      Response response= Nancy.HttpStatusCode.Created;
      return response.WithHeader("Location", location);
    }

    public static Response NotFound(this IResponseFormatter formatter)
    {
      Response response = Nancy.HttpStatusCode.NotFound;
      return response.WithHeader("ReasonPhrase", "Not found");      
    }

    public static Response MovedTo(this IResponseFormatter formatter, string newUri)
    {
      Response response = Nancy.HttpStatusCode.MovedPermanently;
      return response.WithHeaders(
        "ReasonPhrase", "Not found",
        "Location", new Uri(newUri, UriKind.Absolute));
    }

    public static Response NotModified(this IResponseFormatter formatter, TimeSpan? maxAge = null)
    {
      Response response = Nancy.HttpStatusCode.NotModified;
      return response.WithHeaders(
        new Tuple<string, string>("ReasonPhrase", "Not modified"),
        new Tuple<string, string>("Cache-Control", string.Format("max-age={0}, public", maxAge ?? TimeSpan.FromSeconds(10))));
    }

    public static Response AddCacheHeaders(this Response response, IVersionable versionable, TimeSpan? maxAge = null)
    {
      return response.WithHeaders(
        new Tuple<string, string>("ETag", string.Format("\"{0}\"", versionable.Version)),
        new Tuple<string, string>("Cache-Control", string.Format("max-age={0}, public", maxAge ?? TimeSpan.FromSeconds(10))));
    }
  }

  public static class Responses
  {
    public static HttpResponseMessage BadRequest(string reason = "", string content = "")
    {
      return new HttpResponseMessage(HttpStatusCode.BadRequest, reason)
             {
               Content = new StringContent(content)
             };
    }

    public static HttpResponseMessage Created(string uri)
    {
      return new HttpResponseMessage(HttpStatusCode.Created, "Created")
             {
               Headers = {{"Location", uri}}
             };
    }


    public static HttpResponseMessage NotFound()
    {
      return new HttpResponseMessage(HttpStatusCode.NotFound, "Not found");
    }

    public static HttpResponseMessage MovedTo(string newUri)
    {
      return new HttpResponseMessage(HttpStatusCode.MovedPermanently, "MovedPermanently")
             {
               Headers = {Location = new Uri(newUri, UriKind.Absolute)}
             };
    }

    public static HttpResponseMessage WithContent<T>(T content)
    {
      return new HttpResponseMessage<T>(content);
    }

    public static HttpResponseMessage Ok()
    {
      return new HttpResponseMessage(HttpStatusCode.OK, "OK");
    }

    public static HttpResponseMessage NoContent()
    {
      return new HttpResponseMessage(HttpStatusCode.NoContent, "NoContent");
    }

    public static HttpResponseMessage NotModified(TimeSpan? maxAge = null)
    {
      return new HttpResponseMessage(HttpStatusCode.NotModified, "NotModified")
             {
               Headers =
                 {
                   CacheControl = new CacheControlHeaderValue
                                  {
                                    MaxAge = maxAge.HasValue ? maxAge : TimeSpan.FromSeconds(10),
                                    Public = true
                                  }
                 }
             };
    }

    public static HttpResponseMessage AddCacheHeaders(this HttpResponseMessage responseMessage, IVersionable versionable,
                                                      TimeSpan? maxAge = null)
    {
      responseMessage.Headers.CacheControl = new CacheControlHeaderValue
                                             {
                                               Public = true,
                                               MaxAge = maxAge.HasValue ? maxAge : TimeSpan.FromSeconds(10)
                                             };

      responseMessage.Headers.ETag = new EntityTagHeaderValue(string.Format("\"{0}\"", versionable.Version));
      return responseMessage;
    }
  }
}