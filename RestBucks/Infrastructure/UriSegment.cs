namespace RestBucks.Infrastructure
{
  using System;

  public class UriSegment
  {
    private readonly string segment;

    public UriSegment(string segment)
    {
      this.segment = segment;
    }

    public static string operator +(UriSegment baseAddress, string other)
    {
      var baseUri = new Uri(baseAddress.segment);
      return new Uri(baseUri, other).ToString();
    }

    public static string operator +(string baseAddress, UriSegment segment)
    {
      var baseUri = new Uri(baseAddress);
      return new Uri(baseUri, segment.segment).ToString();
    }
  }
}