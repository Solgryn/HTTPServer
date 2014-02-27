using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace HTTPServer
{
    class Program
    {
        static TcpListener serverSocket = new TcpListener(80);

        static void Main(string[] args)
        {
            serverSocket.Start(); //Start listening

            Socket connection = serverSocket.AcceptSocket(); //Accepts a pending connection

            var ns = new NetworkStream(connection);
            var sr = new StreamReader(ns);
            var sw = new StreamWriter(ns);

            while (true)
            {
                var message = sr.ReadLine(); //Get a request
                Console.WriteLine("Client (" + connection.RemoteEndPoint + "): " + message);
            }
        }
    }
}