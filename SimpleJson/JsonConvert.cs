using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace SimpleJson
{
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
                return SerializeObject(value);
            }
        }

        private static object[] GetArray(IList list)
        {
            var resultList = new List<object>();
            foreach (var item in list)
                resultList.Add(ConvertToJsonType(item));
            return resultList.ToArray();
        }

        public static JObject SerializeObject(object obj)
        {
            var json = new JObject();
            var objType = obj.GetType();

            foreach (var field in objType.GetFields())
                json.Add(field.Name, ConvertToJsonType(field.GetValue(obj)));

            foreach (var prop in objType.GetProperties())
                json.Add(prop.Name, ConvertToJsonType(prop.GetValue(obj)));

            return json;
        }

        public static object DeserializeObject(JObject json, Type targetType, params object[] args)
        {
            var obj = Activator.CreateInstance(targetType, args);

            foreach (var field in targetType.GetFields())
            {
                if (!json.Contains(field.Name))
                    continue;

                var value = json[field.Name];
                if (value is JObject jobj)
                    field.SetValue(obj, DeserializeObject(jobj, field.FieldType));
                else
                    field.SetValue(obj, Convert.ChangeType(value, field.FieldType));
            }

            foreach (var prop in targetType.GetProperties())
            {
                if (!json.Contains(prop.Name))
                    continue;

                var value = json[prop.Name];
                if (value is JObject jobj)
                    prop.SetValue(obj, DeserializeObject(jobj, prop.PropertyType));
                else
                    prop.SetValue(obj, Convert.ChangeType(value, prop.PropertyType));
            }

            return obj;
        }

        public static T DeserializeObject<T>(JObject json, params object[] args)
        {
            return (T)DeserializeObject(json, typeof(T), args);
        }
    }
}
