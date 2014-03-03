using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using log4net;

namespace HTTPServer
{
    public class HttpServer
    {
        private readonly int _port;
        private static TcpListener _serverSocket;
        private static readonly ILog Log = LogManager.GetLogger(typeof(HttpServer));

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="port">The port the server listens and responds on.</param>
        public HttpServer(int port)
        {
            _port = port;
        }

        /// <summary>
        /// Runs the server.
        /// </summary>
        public void RunServer()
        {
            _serverSocket = new TcpListener(_port); //Make a new listener
            _serverSocket.Start(); //Start listening
            Log.Info("Server started");

            while (true)
            {
                if (_serverSocket.Pending())
                {
                    Socket connection = _serverSocket.AcceptSocket(); //Accepts a pending connection
                    Log.Info("Client connected");

                    var client = new Client(connection);
                    Task.Run((Action)client.Run); //New client task
                }
            }
        }

        /// <summary>
        /// Stops the server.
        /// </summary>
        public void Stop()
        {
            Task.WaitAll();
            Log.Info("Server stopped");
            _serverSocket.Stop();
            Console.WriteLine("Stopped.");
        }
    }
}
