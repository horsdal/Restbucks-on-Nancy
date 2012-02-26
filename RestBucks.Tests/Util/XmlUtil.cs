using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace RestBucks.Tests.Util
{
    public static class XmlUtil
    {
        public static string ToXmlString<T>(this T input)
        {
            string xml;
            var ser = new XmlSerializer(typeof(T));
            using (var ms = new MemoryStream())
            {
                ser.Serialize(ms, input);
                xml = Encoding.Default.GetString(ms.ToArray());
            }
            return xml;
        }

        public static T FromXmlString<T>(string rawXml)
        {
            var ser = new XmlSerializer(typeof(T));
            using (var ms = new MemoryStream(Encoding.Default.GetBytes(rawXml)))
            {
                return (T)ser.Deserialize(ms);
            }
        }
    }
}