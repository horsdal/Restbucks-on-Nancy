
namespace RestBucks.Infrastructure
{
  using System;

  using Nancy;

  using Domain.BaseClass;

  public static class ResponseHelpers
  {
    public static Response BadRequest(this IResponseFormatter formatter, string reason = "", string content = "")
    {
      Response response = content;
      response.StatusCode = HttpStatusCode.BadRequest;
      return response.WithHeader("ReasonPhrase", reason);
    }

    public static Response Created(this IResponseFormatter formatter, string location)
    {
      Response response= HttpStatusCode.Created;
      return response.WithHeader("Location", location);
    }

    public static Response MovedTo(this IResponseFormatter formatter, string newUri)
    {
      Response response = HttpStatusCode.MovedPermanently;
      return response.WithHeaders(
        new { Header = "ReasonPhrase", Value = "Not found" },
        new { Header = "Location", Value = new Uri(newUri, UriKind.Absolute).ToString() });
    }

    public static Response NotModified(this IResponseFormatter formatter, TimeSpan? maxAge = null)
    {
      Response response = HttpStatusCode.NotModified;
      return response.WithHeaders(
        new { Header = "ReasonPhrase", Value = "Not modified"},
        new { Header = "Cache-Control", Value = string.Format("max-age={0}, public", maxAge ?? TimeSpan.FromSeconds(10)) });
    }

    public static Response AddCacheHeaders(this Response response, IVersionable versionable, TimeSpan? maxAge = null)
    {
      return response.WithHeaders(
        new { Header = "ETag", Value = string.Format("\"{0}\"", versionable.Version) },
        new { Header = "Cache-Control", Value = string.Format("max-age={0}, public", maxAge ?? TimeSpan.FromSeconds(10)) });
    }
  }
}