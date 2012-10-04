using System;
using System.Collections;
using System.Collections.Generic;
using Nancy;
using Nancy.Json;

namespace RestBucks.Infrastructure
{
    /// <summary>
    /// This monstrosity was inspired by this blog post:
    /// http://blog.calyptus.eu/seb/2011/12/custom-datetime-json-serialization/
    /// </summary>
    public class CamelCaseEnumConverter : JavaScriptConverter
    {
        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            // TODO: not sure if this will even work on the way in
            return null;
        }

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            return new HackityHackHack(CamelCase(obj.ToString()));
        }

        private static string CamelCase(string original)
        {
            return original.Substring(0, 1).ToLower() + original.Substring(1);
        }

        public override IEnumerable<Type> SupportedTypes
        {
            get
            {
                return new [] { typeof(Enum) };
            }
        }

        private class HackityHackHack : DynamicDictionaryValue, IDictionary<string, object>
        {
            public HackityHackHack(object value)
                : base(value)
            {
            }

            public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
            {
                throw new NotImplementedException();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }

            public void Add(KeyValuePair<string, object> item)
            {
                throw new NotImplementedException();
            }

            public void Clear()
            {
                throw new NotImplementedException();
            }

            public bool Contains(KeyValuePair<string, object> item)
            {
                throw new NotImplementedException();
            }

            public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
            {
                throw new NotImplementedException();
            }

            public bool Remove(KeyValuePair<string, object> item)
            {
                throw new NotImplementedException();
            }

            public int Count
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public bool IsReadOnly
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public bool ContainsKey(string key)
            {
                throw new NotImplementedException();
            }

            public void Add(string key, object value)
            {
                throw new NotImplementedException();
            }

            public bool Remove(string key)
            {
                throw new NotImplementedException();
            }

            public bool TryGetValue(string key, out object value)
            {
                throw new NotImplementedException();
            }

            public object this[string key]
            {
                get
                {
                    throw new NotImplementedException();
                }
                set
                {
                    throw new NotImplementedException();
                }
            }

            public ICollection<string> Keys
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public ICollection<object> Values
            {
                get
                {
                    throw new NotImplementedException();
                }
            }
        }
    }
}