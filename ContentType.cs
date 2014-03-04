using System.Collections.Generic;
using System.IO;

namespace HTTPServer
{
    public static class ContentType
    {
        private static readonly Dictionary<string, string> types = new Dictionary<string, string>();

        static ContentType()
        {
            types.Add("txt", "text/html");
            types.Add("html", "text/html");
            types.Add("htm", "text/html");
            types.Add("doc", "application/msword");
            types.Add("gif", "image/gif");
            types.Add("jpg", "image/jpeg");
            types.Add("pdf", "application/pdf");
            types.Add("css", "text/css");
            types.Add("xml", "text/xml");
            types.Add("jar", "application/x-java-archive");
        }

        public static string GetContentType(string filename)
        {
            return types[Path.GetExtension(filename).Substring(1)];
        }
    }
}
