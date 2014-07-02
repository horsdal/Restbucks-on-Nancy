namespace RestBucks.Tests.Infrastructure.Linking
{
  using System;
  using NUnit.Framework;
  using RestBucks.Infrastructure.Linking;

#if false
  public class ResourceLinkerTests
  {
    [Test]
    public void Link_generated_is_correct_when_base_uri_has_trailing_slash()
    {
      var resourceLinker = new ResourceLinker("http://localhost/");

      var uriString = resourceLinker.BuildUriString("/foo", "", new {});

      Assert.That(uriString, Is.EqualTo("http://localhost/foo"));
    }

    [Test]
    public void Link_generated_is_correct_when_base_uri_does_not_have_trailing_slash()
    {
      var resourceLinker = new ResourceLinker("http://localhost");

      var uriString = resourceLinker.BuildUriString("/foo", "", new {});

      Assert.That(uriString, Is.EqualTo("http://localhost/foo"));
    }

    [Test]
    public void Link_generated_is_correct_with_simple_template()
    {
      var resourceLinker = new ResourceLinker("http://localhost");

      var uriString = resourceLinker.BuildUriString("/foo", "/bar", new {});

      Assert.That(uriString, Is.EqualTo("http://localhost/foo/bar"));
    }

    [Test]
    public void Link_generated_is_correct_with_bound_parameter()
    {
      var resourceLinker = new ResourceLinker("http://localhost");

      var uriString = resourceLinker.BuildUriString("/foo", "/bar/{id}", new {id = 123});

      Assert.That(uriString, Is.EqualTo("http://localhost/foo/bar/123"));
    }

    [Test]
    public void Argument_exception_is_thrown_if_parameter_from_template_cannot_be_bound()
    {
      var resourceLinker = new ResourceLinker("http://localhost");

      var exception = Assert.Throws<ArgumentException>(() => resourceLinker.BuildUriString("/foo", "/bar/{id}", new {}));

      Assert.That(exception.Message,
                  Is.EqualTo(
                    "The path variable 'ID' in the UriTemplate must be bound to a non-empty string value.\r\nParameter name: parameters"));
    }
  }
  #endif
}