using System.Net.Http.Headers;
using Microsoft.ApplicationServer.Http;
using RestBucks.Resources;

namespace RestBucks.Formatters
{
    public class RestBuckMediaTypeFormatter : XmlMediaTypeFormatter
    {
        public RestBuckMediaTypeFormatter()
        {
            XmlSerializerNamespaces.Add("", "http://restbuckson.net");
            SupportedMediaTypes.Add(new MediaTypeHeaderValue(MediaTypes.Default){CharSet = "utf-8"});
        }
    }
}