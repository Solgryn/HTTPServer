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
        private static readonly string RootCatalog = "c:/temp";

        static void Main(string[] args)
        {
            serverSocket.Start(); //Start listening

            Socket connection = serverSocket.AcceptSocket(); //Accepts a pending connection

            var ns = new NetworkStream(connection);
            var sr = new StreamReader(ns);
            var sw = new StreamWriter(ns);

            var readFirstLine = true; //initialize
            var request = new[] { "" }; //initialize
            while (true)
            {
                var message = sr.ReadLine(); //Get a request
                Console.WriteLine("Client (" + connection.RemoteEndPoint + "): " + message);

                //Get first line from request
                if (readFirstLine)
                {
                    request = message.Split(' '); //split the request up, so the file requested can be read later
                    readFirstLine = false; //don't read anymore lines
                }

                //Send a response when request is done
                if (message == "")
                {
                    //Create HTTP header
                    var response = "";
                    response += "HTTP/1.0 200 OK\r\n";
                    response += "Content-Type: text/html\r\n";
                    response += "\r\n";
                    //Create body from the specified file
                    FileStream fileStream = new FileStream(RootCatalog + request[1], FileMode.Open, FileAccess.Read); //New filestream
                    using (var sr2 = new StreamReader(fileStream)) //use a streamreader to read the filestream
                    {
                        response += sr2.ReadToEnd(); //add contents to the response string
                    }

                    sw.Write(response); //write the response in the streamwriter
                    sw.Flush(); //send the response
                }
            }
        }
    }
}