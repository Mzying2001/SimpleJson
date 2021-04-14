using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleJson
{
    /// <summary>
    /// Static class for reading Json files.
    /// </summary>
    public static class JsonReader
    {
        private static int Next(string str, int index)
        {
            while (char.IsWhiteSpace(str[index]))
                index++;
            return index;
        }

        private static string ReadPropertyName(string str, ref int index)
        {
            index = Next(str, index);
            string propertyName = ReadString(str, ref index);

            while (str[index] != ':')
                index++;
            index++;

            return propertyName;
        }

        private static JObject ReadJsonObject(string str, ref int index)
        {
            index = Next(str, index);
            JObject obj = new JObject();

            if (str[index++] != '{')
                throw new Exception("JObject must start with \"{\".");

            index = Next(str, index);
            if (str[index] == '}')
            {
                index++;
                return obj;
            }

            while (true)
            {
                string propertyName = ReadPropertyName(str, ref index);

                index = Next(str, index);
                switch (str[index])
                {
                    case '{':
                        obj[propertyName] = ReadJsonObject(str, ref index);
                        break;

                    case '[':
                        obj[propertyName] = ReadArray(str, ref index);
                        break;

                    case '"':
                        obj[propertyName] = ReadString(str, ref index);
                        break;

                    default:
                        if (char.IsDigit(str[index]) || str[index] == '+' || str[index] == '-')
                            obj[propertyName] = ReadNumber(str, ref index);
                        else
                            obj[propertyName] = ReadBooleanOrNull(str, ref index);
                        break;
                }

                index = Next(str, index);
                if (str[index] == ',')
                    index++;

                index = Next(str, index);
                if (str[index] == '}')
                    break;
            }

            index++;
            return obj;
        }

        private static string ReadString(string str, ref int index)
        {
            var sb = new StringBuilder();

            if (str[index++] != '"')
                throw new Exception("Strings must be enclosed in quotation marks.");

            while (str[index] != '"')
            {
                char ch = str[index];
                if (ch == '\\')
                {
                    switch (str[++index])
                    {
                        case '\\':
                            ch = '\\';
                            break;

                        case '"':
                            ch = '"';
                            break;

                        case '/':
                            ch = '/';
                            break;

                        case 'f':
                            ch = '\f';
                            break;

                        case 't':
                            ch = '\t';
                            break;

                        case 'n':
                            ch = '\n';
                            break;

                        case 'r':
                            ch = '\r';
                            break;

                        case 'u':
                            ch = (char)Convert.ToInt32(str.Substring(index + 1, 4), 16);
                            index += 4;
                            break;

                        default:
                            throw new Exception($"Invalid escape character: '\\{str[index]}'.");
                    }
                }
                sb.Append(ch);
                index++;
            }

            index++;
            return sb.ToString();
        }

        private static object[] ReadArray(string str, ref int index)
        {
            var list = new List<object>();

            if (str[index++] != '[')
                throw new Exception("Array must start with \"[\".");

            index = Next(str, index);
            if (str[index] == ']')
            {
                index++;
                return list.ToArray();
            }

            while (true)
            {
                char ch = str[index];
                switch (ch)
                {
                    case '{':
                        list.Add(ReadJsonObject(str, ref index));
                        break;

                    case '[':
                        list.Add(ReadArray(str, ref index));
                        break;

                    case '"':
                        list.Add(ReadString(str, ref index));
                        break;

                    default:
                        if (char.IsDigit(str[index]) || str[index] == '+' || str[index] == '-')
                            list.Add(ReadNumber(str, ref index));
                        else
                            list.Add(ReadBooleanOrNull(str, ref index));
                        break;
                }

                index = Next(str, index);
                if (str[index] == ',')
                    index++;

                index = Next(str, index);
                if (str[index] == ']')
                    break;
            }

            index++;
            return list.ToArray();
        }

        private static double ReadNumber(string str, ref int index)
        {
            var sb = new StringBuilder();
            while (char.IsDigit(str[index])
                             || str[index] == '.'
                             || str[index] == '+'
                             || str[index] == '-'
                             || str[index] == 'e'
                             || str[index] == 'E')
                sb.Append(str[index++]);

            return double.Parse(sb.ToString());
        }

        private static object ReadBooleanOrNull(string str, ref int index)
        {
            int tmp = index;
            while (str[index] != ',' && str[index] != ']' && str[index] != '}')
                index++;

            string value = str.Substring(tmp, index - tmp).Trim().ToLower();
            switch (value)
            {
                case "null":
                    return null;

                case "true":
                    return true;

                case "false":
                    return false;

                default:
                    throw new Exception($"Invalid value: {value}.");
            }
        }

        /// <summary>
        /// Converts a string to a JObject.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static JObject Read(string str)
        {
            int index = 0;
            return ReadJsonObject(str, ref index);
        }

        /// <summary>
        /// Reads the specified file and convert it to a JObject.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static JObject ReadFile(string path)
        {
            return Read(System.IO.File.ReadAllText(path));
        }

        /// <summary>
        /// Convert a string representing a json array to object[].
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static object[] ReadArray(string str)
        {
            int index = 0;
            return ReadArray(str.Trim(), ref index);
        }

        /// <summary>
        /// Reads the file representing the json array and convert it to object.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static object[] ReadArrayFile(string path)
        {
            return ReadArray(System.IO.File.ReadAllText(path));
        }
    }
}
