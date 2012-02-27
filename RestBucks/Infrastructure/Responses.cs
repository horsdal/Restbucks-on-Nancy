using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.ApplicationServer.Http;
using RestBucks.Domain.BaseClass;

namespace RestBucks.Infrastructure
{
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