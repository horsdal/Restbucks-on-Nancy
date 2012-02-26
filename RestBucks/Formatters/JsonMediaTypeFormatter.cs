using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using Microsoft.ApplicationServer.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;

namespace RestBucks.Formatters
{
    /// <summary>
    /// Formats requests for text/json and application/json using Json.Net.
    /// </summary>
    /// <remarks>
    /// Christian Weyer is the author of this MediaTypeProcessor.
    /// <see href="http://weblogs.thinktecture.com/cweyer/2010/12/using-jsonnet-as-a-default-serializer-in-wcf-httpwebrest-vnext.html"/>
    /// Daniel Cazzulino (kzu): 
    ///		- updated to support in a single processor both binary and text Json. 
    ///		- fixed to support query composition services properly.
    /// Darrel Miller
    ///     - Converted to Preview 4 MediaTypeFormatter
    /// </remarks>
    public class JsonNetFormatter : MediaTypeFormatter
    {
        private const bool UsesQueryComposition = false;

        public JsonNetFormatter()
        {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/json"));
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/json"));
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/bson"));

            // Operation is no longer available, so I'm not sure how this is going to be supported
            //this.usesQueryComposition = operation.Behaviors.Contains(typeof(QueryCompositionAttribute));
        }

        public override object OnReadFromStream(Type type, Stream stream, HttpContentHeaders contentHeaders)
        {
            var serializer = new JsonSerializer();
            var reader = default(JsonReader);

            if (contentHeaders.ContentType.MediaType == "application/bson")
                reader = new BsonReader(stream);
            else
                reader = new JsonTextReader(new StreamReader(stream));

            var result = serializer.Deserialize(reader, type);

            return result;
        }

        public override void OnWriteToStream(Type type, object value, Stream stream, HttpContentHeaders contentHeaders, TransportContext context)
        {

            var serializer = new JsonSerializer { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
            // NOTE: we don't dispose or close these as they would 
            // close the stream, which is used by the rest of the pipeline.
            var writer = default(JsonWriter);

            if (contentHeaders.ContentType.MediaType == "application/bson")
                writer = new BsonWriter(stream);
            else
                writer = new JsonTextWriter(new StreamWriter(stream));

            if (UsesQueryComposition)
            {
                serializer.Serialize(writer, ((IEnumerable)value).OfType<object>().ToList());
            }
            else
            {
                serializer.Serialize(writer, value);
            }

            writer.Flush();

        }


    }
}
