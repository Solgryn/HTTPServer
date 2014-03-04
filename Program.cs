using log4net.Config;

namespace HTTPServer
{
    /// <summary>
    /// The main program class, creates a server and runs it.
    /// </summary>
    public class Program
    {
        public static int DefaultPort = 80;

        static void Main(string[] args)
        {
            XmlConfigurator.Configure(new System.IO.FileInfo(@"..\..\logconfig.xml"));

            var httpServer = new HttpServer(DefaultPort);
            httpServer.RunServer();
        }
    }
}