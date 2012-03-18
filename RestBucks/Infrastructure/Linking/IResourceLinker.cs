namespace RestBucks.Infrastructure.Linking
{
    public interface IResourceLinker
    {
      string BuildUriString(string prefix, string template, dynamic parameters);
    }
}