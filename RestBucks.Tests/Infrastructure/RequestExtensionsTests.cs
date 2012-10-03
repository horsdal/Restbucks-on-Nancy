using NUnit.Framework;
using Nancy;
using RestBucks.Infrastructure;

namespace RestBucks.Tests.Infrastructure
{
    public class RequestExtensionsTests
    {
        [Test]
        public void Base_uri_should_contain_request_scheme()
        {
            var request = new Request("method", "path", "scheme");

            var baseUri = request.BaseUri();

            Assert.That(baseUri, Is.StringStarting("scheme://"));
        }

        [Test]
        public void Base_uri_should_contain_request_host()
        {
            var request = new Request("method", new Url { HostName = "host", Path = "path" });

            var baseUri = request.BaseUri();

            Assert.That(baseUri, Is.StringContaining("http://host"));
        }

        [Test]
        public void Base_uri_should_contain_request_port_if_present()
        {
            var request = new Request("method", new Url { HostName = "host", Port = 8000, Path = "path" });

            var baseUri = request.BaseUri();

            Assert.That(baseUri, Is.StringContaining("http://host:8000"));
        }

        [Test]
        public void Base_uri_should_contain_bogus_if_host_is_not_provided()
        {
            var request = new Request("method", new Url { HostName = "", Path = "path" });

            var baseUri = request.BaseUri();

            Assert.That(baseUri, Is.StringContaining("http://bogus"));
        }

        [Test]
        public void Base_uri_should_contain_request_base_path()
        {
            var request = new Request("method", new Url { HostName = "host", Path = "path", BasePath = "/basePath" });

            var baseUri = request.BaseUri();

            Assert.That(baseUri, Is.StringContaining("http://host/basePath"));
        }
    }
}