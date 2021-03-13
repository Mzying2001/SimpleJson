using System.Text;

namespace SimpleJson
{
    public static class JsonWriter
    {
        private static void WriteObject(StringBuilder sb, object value)
        {
            if (value == null)
            {
                sb.Append("null");
            }
            else if (value is JObject json)
            {
                WriteJsonObject(sb, json);
            }
            else if (value is object[] arr)
            {
                WriteArray(sb, arr);
            }
            else if (value is string || value is StringBuilder)
            {
                WriteString(sb, value.ToString());
            }
            else if (value is bool || double.TryParse(value.ToString(), out _))
            {
                sb.Append(value.ToString().ToLower());
            }
            else
            {
                sb.Append(value.ToString());
            }
        }

        private static void WriteJsonObject(StringBuilder sb, JObject json)
        {
            sb.Append('{');
            foreach (var item in json)
            {
                sb.Append($"\"{item.Key}\":");
                WriteObject(sb, item.Value);
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
                WriteObject(sb, item);
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
            if (json == null)
                return null;

            var sb = new StringBuilder();
            WriteJsonObject(sb, json);
            return sb.ToString();
        }

        public static void WriteFile(string path, JObject json)
        {
            System.IO.File.WriteAllText(path, JsonToString(json));
        }
    }
}
