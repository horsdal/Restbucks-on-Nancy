namespace RestBucks.Infrastructure
{
  using System.Configuration;

  public static class BaseAddress
  {
    public static string Current
    {
      get { return ConfigurationManager.AppSettings["baseUri"] ?? ""; }
    }
  }
}