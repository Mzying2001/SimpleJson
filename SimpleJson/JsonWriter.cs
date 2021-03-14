using System.Text;

namespace SimpleJson
{
    public static class JsonWriter
    {
        private static void Indent(StringBuilder sb, int indent)
        {
            while (indent-- > 0)
                sb.Append("    ");
        }

        private static void WriteObject(StringBuilder sb, object value, int indent)
        {
            if (value == null)
            {
                sb.Append("null");
            }
            else if (value is JObject json)
            {
                WriteJsonObject(sb, json, indent);
            }
            else if (value is object[] arr)
            {
                WriteArray(sb, arr, indent);
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
                WriteString(sb, value.ToString());
            }
        }

        private static void WriteJsonObject(StringBuilder sb, JObject json, int indent)
        {
            bool first = true;

            sb.Append('{');
            if (json.Count == 0)
            {
                sb.Append('}');
                return;
            }

            sb.Append('\n');
            foreach (var item in json)
            {
                if (first)
                    first = false;
                else
                    sb.Append(",\n");

                Indent(sb, indent + 1);
                sb.Append($"\"{item.Key}\": ");
                WriteObject(sb, item.Value, indent + 1);
            }
            sb.Append('\n');
            Indent(sb, indent);
            sb.Append('}');
        }

        private static void WriteArray(StringBuilder sb, object[] arr, int indent)
        {
            bool first = true;

            sb.Append('[');
            if (arr.Length == 0)
            {
                sb.Append(']');
                return;
            }

            sb.Append('\n');
            foreach (var item in arr)
            {
                if (first)
                    first = false;
                else
                    sb.Append(",\n");

                Indent(sb, indent + 1);
                WriteObject(sb, item, indent + 1);
            }
            sb.Append('\n');
            Indent(sb, indent);
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

        public static string Write(JObject json)
        {
            if (json == null)
                return string.Empty;

            var sb = new StringBuilder();
            WriteJsonObject(sb, json, 0);
            return sb.ToString();
        }

        public static void WriteFile(string path, JObject json)
        {
            System.IO.File.WriteAllText(path, Write(json));
        }
    }
}
