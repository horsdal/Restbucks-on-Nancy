namespace RestBucks.Tests
{
  using NUnit.Framework;

  using Nancy.Testing;

  using RestBucks;

  [TestFixture]
  public class IntegrationSmokeTests
  {
    [Test]
    public void AppCanInitializeWithRealDependencies()
    {
      Assert.DoesNotThrow(
        () => new Browser(new Bootstrapper())
        );
    }
  }
}
