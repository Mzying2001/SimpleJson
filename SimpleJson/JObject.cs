using System;
using System.Collections;
using System.Collections.Generic;

namespace SimpleJson
{
    public class JObject : IEnumerable<KeyValuePair<string, object>>
    {
        protected Dictionary<string, object> Content { get; set; }

        public int Count => Content.Count;

        public IEnumerable<string> PropertyNames
        {
            get => Content.Keys;
        }

        public IEnumerable<object> Values
        {
            get => Content.Values;
        }

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
            return JsonWriter.Write(this);
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
                if (Contains(propertyName))
                    Content[propertyName] = value;
                else
                    Add(propertyName, value);
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

        public bool TryGetValue<T>(string propertyName, out T value)
        {
            try
            {
                value = GetValue<T>(propertyName);
                return true;
            }
            catch
            {
                value = default;
                return false;
            }
        }

        public T[] GetArray<T>(string propertyName)
        {
            var source = GetValue<object[]>(propertyName);
            if (typeof(T) == typeof(object))
                return (T[])(object)source;

            var result = new T[source.Length];
            for (int i = 0; i < source.Length; i++)
                result[i] = (T)source[i];
            return result;
        }

        public bool TryGetArray<T>(string propertyName, out T[] value)
        {
            try
            {
                value = GetArray<T>(propertyName);
                return true;
            }
            catch
            {
                value = default;
                return false;
            }
        }

        public void Add(string propertyName, object value)
        {
            if (Contains(propertyName))
                throw new Exception($"Prpperty \"{propertyName}\" already exists.");
            Content.Add(propertyName, value);
        }

        public void Add<T>(KeyValuePair<string, T> keyValuePair)
        {
            Add(keyValuePair.Key, keyValuePair.Value);
        }

        public void Add(params KeyValuePair<string, object>[] keyValuePairs)
        {
            foreach (var item in keyValuePairs)
                Add(item.Key, item.Value);
        }

        public void Add<T>(IEnumerable<KeyValuePair<string, T>> keyValuePairs)
        {
            foreach (var item in keyValuePairs)
                Add(item.Key, item.Value);
        }

        public void Add<T>((string, T) tuple)
        {
            Add(tuple.Item1, tuple.Item2);
        }

        public void Add(params (string, object)[] tuples)
        {
            foreach (var item in tuples)
                Add(item.Item1, item.Item2);
        }

        public void Add<T>(IEnumerable<(string, T)> tuples)
        {
            foreach (var item in tuples)
                Add(item.Item1, item.Item2);
        }

        public void Clear()
        {
            Content.Clear();
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
