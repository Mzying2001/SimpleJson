using System;
using System.Collections;
using System.Collections.Generic;

namespace SimpleJson
{
    public class JObject : IEnumerable<KeyValuePair<string, object>>
    {
        protected Dictionary<string, object> Content { get; set; }

        /// <summary>
        /// Get the number of elements in the JObject.
        /// </summary>
        public int Count => Content.Count;

        /// <summary>
        /// Get an enumerator of all element names in the JObject.
        /// </summary>
        public IEnumerable<string> PropertyNames
        {
            get => Content.Keys;
        }

        /// <summary>
        /// Get an enumerator of all values in the JObject.
        /// </summary>
        public IEnumerable<object> Values
        {
            get => Content.Values;
        }

        /// <summary>
        /// Initializes a JObject instance.
        /// </summary>
        public JObject()
        {
            Content = new Dictionary<string, object>();
        }

        /// <summary>
        /// Determines if the JObject contains a certain element.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Get or set the value by element name.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Get or set the value of a child JObject by element name.
        /// </summary>
        /// <param name="propertyNames"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Get the element of the specified type by element name.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public T GetValue<T>(string propertyName)
        {
            var value = this[propertyName];
            return value == null ? default : (T)Convert.ChangeType(value, typeof(T));
        }

        /// <summary>
        /// Attempts to get the element of the specified type by element name.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        /// <returns>Returns whether the value was successfully retrieved.</returns>
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

        /// <summary>
        /// Get an array of the specified type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyName"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Attempts to get an array of the specified type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        /// <returns>Returns if the array was successfully obtained.</returns>
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

        /// <summary>
        /// Add element.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        public void Add(string propertyName, object value)
        {
            if (Contains(propertyName))
                throw new Exception($"Prpperty \"{propertyName}\" already exists.");
            Content.Add(propertyName, value);
        }

        /// <summary>
        /// Adds an element via KeyValuePair.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keyValuePair"></param>
        public void Add<T>(KeyValuePair<string, T> keyValuePair)
        {
            Add(keyValuePair.Key, keyValuePair.Value);
        }

        /// <summary>
        /// Adds an element via KeyValuePairs.
        /// </summary>
        /// <param name="keyValuePairs"></param>
        public void Add(params KeyValuePair<string, object>[] keyValuePairs)
        {
            foreach (var item in keyValuePairs)
                Add(item.Key, item.Value);
        }

        /// <summary>
        /// Adds an element via KeyValuePairs.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keyValuePairs"></param>
        public void Add<T>(IEnumerable<KeyValuePair<string, T>> keyValuePairs)
        {
            foreach (var item in keyValuePairs)
                Add(item.Key, item.Value);
        }

        /// <summary>
        /// Adds elements via tuple.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tuple"></param>
        public void Add<T>((string, T) tuple)
        {
            Add(tuple.Item1, tuple.Item2);
        }

        /// <summary>
        /// Adds elements via tuples.
        /// </summary>
        /// <param name="tuples"></param>
        public void Add(params (string, object)[] tuples)
        {
            foreach (var item in tuples)
                Add(item.Item1, item.Item2);
        }

        /// <summary>
        /// Adds elements via tuples.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tuples"></param>
        public void Add<T>(IEnumerable<(string, T)> tuples)
        {
            foreach (var item in tuples)
                Add(item.Item1, item.Item2);
        }

        /// <summary>
        /// Removes all keys and values of the JObject.
        /// </summary>
        public void Clear()
        {
            Content.Clear();
        }

        /// <summary>
        /// Removes the element with the specified name.
        /// </summary>
        /// <param name="propertyName"></param>
        public void Remove(string propertyName)
        {
            Content.Remove(propertyName);
        }

        /// <summary>
        /// Parse string into JObject.
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static JObject Parse(string json)
        {
            return JsonReader.Read(json);
        }
    }
}
