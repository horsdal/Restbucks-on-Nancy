using System.Linq;
using System.Net.Http;
using RestBucks.Domain.BaseClass;

namespace RestBucks.Infrastructure
{
    public static class RequestExtensions
    {
        public static bool IsNotModified(this HttpRequestMessage requestMessage, IVersionable versionable)
        {
            if (!requestMessage.Headers.IfNoneMatch.Any()) return false;
            var etag = requestMessage.Headers.IfNoneMatch.First().Tag;
            return string.Format("\"{0}\"", versionable.Version) == etag;
        }
    }
}