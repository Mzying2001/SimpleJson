using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleJson
{
    public static class JsonWriter
    {
        private static void WriteJObject(StringBuilder sb, JObject json)
        {
            sb.Append('{');
            foreach (var item in json)
            {
                sb.Append($"\"{item.Key}\":");
                var value = item.Value;
                if (value is JObject obj)
                {
                    WriteJObject(sb, obj);
                }
                else if (value is object[] arr)
                {
                    WriteArray(sb, arr);
                }
                else if (value is string str)
                {
                    WriteString(sb, str);
                }
                else
                {
                    sb.Append(value.ToString());
                }
                sb.Append(",");
            }
            if (sb[sb.Length - 1] == ',')
                sb[sb.Length - 1] = '}';
            else
                sb.Append('}');
        }

        private static void WriteArray(StringBuilder sb, object[] arr)
        {
            sb.Append('[');
            foreach (var item in arr)
            {
                if (item is JObject obj)
                {
                    WriteJObject(sb, obj);
                }
                else if (item is object[] objs)
                {
                    WriteArray(sb, objs);
                }
                else if (item is string || item is StringBuilder)
                {
                    WriteString(sb, item.ToString());
                }
                else if (item is bool || double.TryParse(item.ToString(), out _))
                {
                    sb.Append(item.ToString());
                }
                else
                {
                    WriteString(sb, item.ToString());
                }
                sb.Append(",");
            }
            if (sb[sb.Length - 1] == ',')
                sb[sb.Length - 1] = ']';
            else
                sb.Append(']');
        }

        private static void WriteString(StringBuilder sb, string str)
        {
            sb.Append('"');
            foreach (char c in str)
            {
                switch (c)
                {
                    case '\\':
                        sb.Append("\\\\");
                        break;

                    case '\"':
                        sb.Append("\\\"");
                        break;

                    case '\f':
                        sb.Append("\\f");
                        break;

                    case '\t':
                        sb.Append("\\t");
                        break;

                    case '\n':
                        sb.Append("\\n");
                        break;

                    case '\r':
                        sb.Append("\\r");
                        break;

                    default:
                        sb.Append(c);
                        break;
                }
            }
            sb.Append('"');
        }

        public static string JsonToString(JObject json)
        {
            var sb = new StringBuilder();
            WriteJObject(sb, json);
            return sb.ToString();
        }
    }
}
