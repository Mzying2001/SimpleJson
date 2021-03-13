using System;
using System.Collections;
using System.Collections.Generic;

namespace SimpleJson
{
    public class JObject : IEnumerable<KeyValuePair<string, object>>
    {
        public Dictionary<string, object> Content { get; private set; }

        public int Count => Content.Count;

        public JObject()
        {
            Content = new Dictionary<string, object>();
        }

        public bool Contains(string propertyName)
        {
            return Content.ContainsKey(propertyName);
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

        public object this[params string[] propertyNames]
        {
            get
            {
                var json = this;
                int count = propertyNames.Length;
                for (int i = 0; i < count - 1; i++)
                {
                    json = json[propertyNames[i]] as JObject;
                }
                return json[propertyNames[count - 1]];
            }
            set
            {
                var json = this;
                int count = propertyNames.Length;
                for (int i = 0; i < count - 1; i++)
                {
                    if (json.Contains(propertyNames[i]))
                        json = json[propertyNames[i]] as JObject;
                    else
                    {
                        var next = new JObject();
                        json[propertyNames[i]] = next;
                        json = next;
                    }
                }
                json[propertyNames[count - 1]] = value;
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
