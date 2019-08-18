using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace EclassApi.Extensions
{
    public static class StringExtensions
    {
        private static readonly string HTML_TAG_PATTERN = "</*?>";
        private static readonly string[] ESCAPE_CHARACTERS =
        {
            "\"", "\\", "\a", "\b", "\r", "\n", "\t"
        };
        public static string HTMLtoSTRING(this string HTML)
        {
            string tmp = HTML;
            tmp = Regex.Replace
              (tmp, HTML_TAG_PATTERN, string.Empty);
            tmp = Regex.Replace(tmp, "&#", string.Empty);
            tmp = Regex.Replace(tmp, "160;", string.Empty);
            return tmp;
        }

        public static string RemoveEscapeChars(this string str)
        {
            var tmp = str;
            foreach(string escChar in ESCAPE_CHARACTERS)
            {
                tmp = tmp.Replace(escChar, string.Empty);
            }
            return tmp;
        }

        public static string IfNullConvertToEmpty(string str)
        {
            if (str == null)
                return string.Empty;
            return str;
        }

        public static Stream GenerateStreamFromString(string s)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
    }
}
