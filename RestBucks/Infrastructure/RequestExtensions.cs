namespace RestBucks.Infrastructure
{
  using System.Linq;

  using Nancy;

  using Domain.BaseClass;

  public static class RequestExtensions
  {
    public static bool IsNotModified(this Request requestMessage, IVersionable versionable)
    {
      if (!requestMessage.Headers.IfNoneMatch.Any()) return false;
      var etag = requestMessage.Headers.IfNoneMatch.First();
      return string.Format("\"{0}\"", versionable.Version) == etag;
    }
  }
}