using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HTTPServer
{
    static class ContentType
    {
        private static Dictionary<string, string> types = new Dictionary<string, string>();

        static ContentType()
        {
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
