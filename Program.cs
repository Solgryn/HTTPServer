using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HTTPServer
{
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