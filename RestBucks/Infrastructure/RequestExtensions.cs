namespace RestBucks.Infrastructure
{
  using System.Linq;

  using Nancy;

  using Domain.BaseClass;

  public static class RequestExtensions
  {
    public static bool IsNotModified(this Request request, IVersionable versionable)
    {
      if (!request.Headers.IfNoneMatch.Any()) return false;
      var etag = request.Headers.IfNoneMatch.First();
      return string.Format("\"{0}\"", versionable.Version) == etag;
    }

    public static string BaseUri(this Request request)
    {
      var host = request.Url.Port == null
                   ? request.Url.HostName
                   : string.Format("{0}:{1}", request.Url.HostName, request.Url.Port);
      return string.Format("{0}://{1}/", request.Url.Scheme, string.IsNullOrWhiteSpace(host) ? "bogus" : host);
    }

  }
}