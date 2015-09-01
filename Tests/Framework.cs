using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace TinyXmlMapper.Tests
{
    class Framework
    {
        internal static Stream LoadInternal<T>(string name)
        {
            var assem = typeof(T).Assembly;
            return assem.GetManifestResourceStream(name);
        }

        internal static string LoadInternalAsString<T>(string name)
        {
            using (var resource = LoadInternal<T>(name))
            {
                using (StreamReader reader = new StreamReader(resource, Encoding.UTF8))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        internal static string ReplaceWhitespace(string xmlString)
        {
            xmlString = Regex.Replace(xmlString, @"\r\n?|\n", "");
            xmlString = Regex.Replace(xmlString, @">\s+<", "><");
            return xmlString;
        }
    }
}
