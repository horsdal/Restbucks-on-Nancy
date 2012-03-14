using System.Linq;

using RestBucks.Domain.BaseClass;

namespace RestBucks.Infrastructure
{
  using Nancy;

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