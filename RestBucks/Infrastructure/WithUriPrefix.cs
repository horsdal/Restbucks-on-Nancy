using System;

namespace RestBucks.Infrastructure
{
    public class WithUriPrefix : Attribute
    {
        public WithUriPrefix(string uri)
        {
            Uri = uri;
        }

        public string Uri { get; private set; }
    }
}