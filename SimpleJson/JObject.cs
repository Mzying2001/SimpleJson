using System;
using System.Collections;
using System.Collections.Generic;

namespace SimpleJson
{
    public class JObject : IEnumerable<KeyValuePair<string, object>>
    {
        public Dictionary<string, object> Content { get; private set; }

        public JObject()
        {
            Content = new Dictionary<string, object>();
        }

        public override string ToString()
        {
            return JsonWriter.JsonToString(this);
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return Content.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public object this[string propertyName]
        {
            get => Content[propertyName];
            set
            {
                if (Content.ContainsKey(propertyName))
                    Content[propertyName] = value;
                else
                    Content.Add(propertyName, value);
            }
        }

        public T GetValue<T>(string propertyName)
        {
            var value = this[propertyName];
            return value == null ? default : (T)Convert.ChangeType(value, typeof(T));
        }

        public void Remove(string propertyName)
        {
            Content.Remove(propertyName);
        }

        public static JObject Parse(string json)
        {
            return JsonReader.Read(json);
        }
    }
}
