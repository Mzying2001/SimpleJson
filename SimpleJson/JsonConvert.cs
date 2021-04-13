using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace SimpleJson
{
    /// <summary>
    /// Static class for serializing and deserializing Json.
    /// </summary>
    public static class JsonConvert
    {
        private static object ConvertToJsonType(object value)
        {
            if (value == null)
            {
                return null;
            }
            else if (value is IList list)
            {
                return GetArray(list);
            }
            else if (value is string || value is StringBuilder)
            {
                return value.ToString();
            }
            else if (value is JObject || value is bool || double.TryParse(value.ToString(), out _))
            {
                return value;
            }
            else
            {
                return Serialize(value);
            }
        }

        private static object[] GetArray(IList list)
        {
            var resultList = new List<object>();
            foreach (var item in list)
                resultList.Add(ConvertToJsonType(item));
            return resultList.ToArray();
        }

        /// <summary>
        /// Serializes the specified object instance into a JObject.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static JObject Serialize(object obj)
        {
            var json = new JObject();
            var objType = obj.GetType();

            foreach (var field in objType.GetFields())
                json.Add(field.Name, ConvertToJsonType(field.GetValue(obj)));

            foreach (var prop in objType.GetProperties())
                json.Add(prop.Name, ConvertToJsonType(prop.GetValue(obj)));

            return json;
        }

        /// <summary>
        /// Attempts to serialize the specified object instance into a JObject.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="json"></param>
        /// <returns>Returns if the serialization was successful.</returns>
        public static bool TrySerialize(object obj, out JObject json)
        {
            try
            {
                json = Serialize(obj);
                return true;
            }
            catch
            {
                json = null;
                return false;
            }
        }

        /// <summary>
        /// Deserializes a JObject into an object of the specified type.
        /// </summary>
        /// <param name="json"></param>
        /// <param name="targetType"></param>
        /// <param name="args">The parameter to initialize the object.</param>
        /// <returns></returns>
        public static object Deserialize(JObject json, Type targetType, params object[] args)
        {
            var obj = Activator.CreateInstance(targetType, args);

            foreach (var field in targetType.GetFields())
            {
                if (!json.Contains(field.Name))
                    continue;

                var value = json[field.Name];
                var fType = field.FieldType;

                if (value is JObject jobj)
                    field.SetValue(obj, fType == typeof(JObject) ? value : Deserialize(jobj, fType));
                else
                    field.SetValue(obj, Convert.ChangeType(value, fType));
            }

            foreach (var prop in targetType.GetProperties())
            {
                if (!json.Contains(prop.Name))
                    continue;

                var value = json[prop.Name];
                var pType = prop.PropertyType;

                if (value is JObject jobj)
                    prop.SetValue(obj, pType == typeof(JObject) ? value : Deserialize(jobj, pType));
                else
                    prop.SetValue(obj, Convert.ChangeType(value, pType));
            }

            return obj;
        }

        /// <summary>
        /// Deserializes a JObject into an object of the specified type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <param name="args">The parameter to initialize the object.</param>
        /// <returns></returns>
        public static T Deserialize<T>(JObject json, params object[] args)
        {
            return (T)Deserialize(json, typeof(T), args);
        }

        /// <summary>
        /// Deserializes a json string into an object of the specified type.
        /// </summary>
        /// <param name="jsonStr"></param>
        /// <param name="targrtType"></param>
        /// <param name="args">The parameter to initialize the object.</param>
        /// <returns></returns>
        public static object Deserialize(string jsonStr, Type targrtType, params object[] args)
        {
            var json = JsonReader.Read(jsonStr);
            return Deserialize(json, targrtType, args);
        }

        /// <summary>
        /// Deserializes a json string into an object of the specified type.
        /// </summary>
        /// <param name="jsonStr"></param>
        /// <param name="args">The parameter to initialize the object.</param>
        /// <returns></returns>
        public static T Deserialize<T>(string jsonStr, params object[] args)
        {
            var json = JsonReader.Read(jsonStr);
            return Deserialize<T>(json, args);
        }

        /// <summary>
        /// Attempts to deserialize a JObject into an object instance of the specified type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <param name="value"></param>
        /// <param name="args">The parameter to initialize the object.</param>
        /// <returns>Returns whether the deserialization was successful.</returns>
        public static bool TryDeserialize<T>(JObject json, out T value, params object[] args)
        {
            try
            {
                value = Deserialize<T>(json, args);
                return true;
            }
            catch
            {
                value = default;
                return false;
            }
        }

        /// <summary>
        /// Attempts to deserialize a json string into an object instance of the specified type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonStr"></param>
        /// <param name="value"></param>
        /// <param name="args">The parameter to initialize the object.</param>
        /// <returns>Returns whether the deserialization was successful.</returns>
        public static bool TryDeserialize<T>(string jsonStr, out T value, params object[] args)
        {
            try
            {
                var json = JsonReader.Read(jsonStr);
                value = Deserialize<T>(json, args);
                return true;
            }
            catch
            {
                value = default;
                return false;
            }
        }
    }
}
