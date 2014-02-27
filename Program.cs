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

            var readFirstLine = true;
            var request = new[] { "" };
            while (true)
            {
                var message = sr.ReadLine(); //Get a request
                Console.WriteLine("Client (" + connection.RemoteEndPoint + "): " + message);

                //Get request
                if (readFirstLine)
                {
                    request = message.Split(' ');
                    readFirstLine = false;
                }

                //Send a response when request is done
                if (message == "")
                {
                    var response = "";
                    response += "HTTP/1.0 200 OK\r\n";
                    response += "Content-Type: text/html\r\n";
                    response += "\r\n";
                    response += "<html><body>";
                    response += "<b>Hello, you wrote " + request[1] + "</b>";
                    response += "</body></html>";
                    sw.Write(response);
                    sw.Flush();
                }
            }
        }
    }
}