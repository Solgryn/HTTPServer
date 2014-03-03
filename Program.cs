using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using log4net;
using log4net.Config;

namespace HTTPServer
{
    /// <summary>
    /// The main program class, accepting connections and creating new client threads.
    /// </summary>
    public class Program
    {
        private static readonly ILog log = LogManager.GetLogger(typeof (Program));
        public static int DefaultPort = 80;
        static readonly TcpListener ServerSocket = new TcpListener(DefaultPort);

        static void Main(string[] args)
        {
            XmlConfigurator.Configure(new System.IO.FileInfo(@"..\..\logconfig.xml"));

            ServerSocket.Start(); //Start listening
            log.Info("Server started");

            while (true)
            {
                if (ServerSocket.Pending())
                {
                    Socket connection = ServerSocket.AcceptSocket(); //Accepts a pending connection
                    log.Info("Client connected");

                    var client = new Client(connection);
                    Task.Run((Action) client.Run); //New client task
                }
            }
        }
    }
}