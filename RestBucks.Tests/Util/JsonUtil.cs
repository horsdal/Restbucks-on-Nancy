using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using Newtonsoft.Json;

namespace RestBucks.Tests.Util
{
    public static class JsonUtil
    {
        public static string ToJsonString<T>(this T input)
        {
            string xml;
            var serializer = new JsonSerializer();
            using (var ms = new MemoryStream())
            using(var tw = new StreamWriter(ms))
            using(var jswriter = new JsonTextWriter(tw))
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