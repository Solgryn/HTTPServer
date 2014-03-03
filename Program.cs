using System;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace HTTPServer
{
    /// <summary>
    /// The main program class, accepting connections and creating new client threads.
    /// </summary>
    public class Program
    {
        public static int DefaultPort = 80;
        static readonly TcpListener ServerSocket = new TcpListener(DefaultPort);

        static void Main(string[] args)
        {
            ServerSocket.Start(); //Start listening
            Console.WriteLine("Started listening.");

            while (true)
            {
                if (ServerSocket.Pending())
                {
                    Socket connection = ServerSocket.AcceptSocket(); //Accepts a pending connection
                    Console.WriteLine("Socket accepted.");

                    var client = new Client(connection);
                    Task.Factory.StartNew(client.Run); //New client task
                }
            }
        }
    }
}