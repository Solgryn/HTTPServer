using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace HTTPServer
{
    class Client
    {
        private Socket connection;
        private static readonly string RootCatalog = "c:/temp";

        public Client(Socket connection)
        {
            this.connection = connection;
        }

        public void Run()
        {
            var response = "";
            var ns = new NetworkStream(connection);
            var sr = new StreamReader(ns);
            var sw = new StreamWriter(ns);

            var readFirstLine = true; //initialize
            var request = new[] { "" }; //initialize
            while (true)
            {
                var message = sr.ReadLine(); //Get a request
                if (message != null)
                {
                    Console.WriteLine("Client (" + connection.RemoteEndPoint + "): " + message);
                }

                //Get first line from request
                if (readFirstLine && message != null)
                {
                    request = message.Split(' '); //split the request up, so the file requested can be read later
                    readFirstLine = false; //don't read anymore lines
                }

                //Send a response when request is done
                if (message == "")
                {
                    //if (Regex.IsMatch(request, @"GET /[a-z] HTTP/1.0"))
                    //{
                        //Does the file exist?
                        if (File.Exists(RootCatalog + request[1]))
                        {
                            //Create HTTP header
                            response += "HTTP/1.0 200 OK\r\n";
                            response += "Content-Type: text/html\r\n";
                            response += "\r\n";
                            //Create body from the specified file
                            FileStream fileStream = new FileStream(RootCatalog + request[1], FileMode.Open,
                                FileAccess.Read);
                            //New filestream
                            using (var sr2 = new StreamReader(fileStream)) //use a streamreader to read the filestream
                            {
                                response += sr2.ReadToEnd(); //add contents to the response string
                            }
                            break;
                        }
                        else //If the file doesn't exist, return "404 Not Found"
                        {
                            Console.WriteLine("Not found.");
                            //Create HTTP 404 header
                            response += "HTTP/1.0 404 Not Found\r\n";
                            response += "\r\n";
                            response += "<html>404 not found.</html>";
                            break;
                        }
                    /*}
                    else
                    {
                        //Create HTTP 40 header
                        var response = "";
                        response += "HTTP/1.0 400 Illegal request\r\n";
                        response += "\r\n";
                    }*/
                }
            }
            try
            {
                sw.Write(response); //write the response in the streamwriter
                sw.Flush(); //send the response
                Console.WriteLine("Sent response.");
            }
            finally
            {
                ns.Close();
                Console.WriteLine("Closed connection.");
            }
        }
    }
}
