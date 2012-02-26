using System.Net.Http;

namespace RestBucks.Tests.Util
{
    public static class HttpContentExtensions
    {
        public static string ToStringContent(this HttpContent content)
        {
            var stringContent = (StringContent) content;
            return stringContent.ReadAsString();
        }
    }
}