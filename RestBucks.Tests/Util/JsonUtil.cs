namespace RestBucks.Tests.Util
{
  using System.IO;
  using System.Text;

  using Newtonsoft.Json;

  public static class JsonUtil
  {
    public static string ToJsonString<T>(this T input)
    {
      string xml;
      var serializer = new JsonSerializer();
      using (var ms = new MemoryStream())
      using (var tw = new StreamWriter(ms))
      using (var jswriter = new JsonTextWriter(tw))
      {
        serializer.Serialize(jswriter, input);
        jswriter.Flush();
        ms.Position = 0;
        xml = Encoding.Default.GetString(ms.ToArray());
      }
      return xml;
    }
  }
}