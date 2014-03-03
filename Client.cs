using System;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using log4net;

namespace HTTPServer
{
    /// <summary>
    /// The client class, a connection to a browser client.
    /// </summary>
    class Client
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(Client));
        private string _response = "";
        private readonly Socket _connection;
        private const string RootCatalog = "c:/temp";
        private static readonly string[] Methods = {"GET", "POST"};

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="connection">The socket connection.</param>
        public Client(Socket connection)
        {
            _connection = connection;
        }

        /// <summary>
        /// Runs the client connection.
        /// </summary>
        public void Run()
        {
            var ns = new NetworkStream(_connection);
            var sr = new StreamReader(ns);
            var sw = new StreamWriter(ns);

            byte[] ar = {};
            var readFirstLine = true; //initialize
            var requestStr = "";
            var request = new[] { "" }; //initialize
            while (true)
            {
                var message = sr.ReadLine(); //Get a request
                if (message != null)
                {
                    Log.Info("Client (" + _connection.RemoteEndPoint + ") message: '" + message);
                }

                //Get first line from request
                if (readFirstLine && message != null)
                {
                    requestStr = message;
                    request = message.Split(' '); //split the request up, so the file requested can be read later
                    readFirstLine = false; //don't read anymore lines
                }

                //Send a response when request is done
                if (message == "")
                {
                    var filename = request[1];
                    //Is the request in the correct format?
                    if (Regex.IsMatch(requestStr, @"^[A-Z]{3,4} /.{0,150} HTTP/[0-9]\.[0-9]$") && Methods.Contains(request[0]))
                    {
                        //Is it an illegal protocol?
                        if (request[2] == "HTTP/1.2")
                        {
                            Log.Info("Server response: Illegal protocol");
                            //Create HTTP 404 header
                            _response += "HTTP/1.0 400 Illegal protocol\r\n";
                            _response += "\r\n";
                            _response += "<html>400 Illegal protocol</html>";
                            break;
                        }
                        //Does the file exist?
                        if (File.Exists(RootCatalog + request[1]))
                        {
                            //Is the request POST? (Only get implemented at the moment)
                            if (request[0] == "POST")
                            {
                                //Create HTTP not yet implemented header
                                _response += "HTTP/1.0 200 xxx\r\n";
                                _response += "\r\n";
                                break;
                            }

                            Log.Info("Server response: OK, Content-type: " + ContentType.GetContentType(filename));
                            var fileStream = new FileStream(RootCatalog + filename, FileMode.Open, FileAccess.Read); //Initialize a filestream
                            //Create HTTP header
                            _response += "HTTP/1.0 200 OK\r\n";
                            _response += "Content-Type: " + ContentType.GetContentType(filename) + "\r\n";
                            _response += "Content-Length: " + fileStream.Length + "\r\n";
                            _response += "Date: " + DateTime.Now.Date.ToUniversalTime().ToString("r") +"\r\n";
                            _response += "\r\n";
                            //Create body from the specified file
                            switch (ContentType.GetContentType(filename)) // depending on the type
                            {
                                case "image/jpeg": //if the file is image/jpeg use the filestream
                                    using (fileStream)
                                    {
                                        ar = new byte[fileStream.Length];
                                        fileStream.Read(ar, 0, (int) fileStream.Length);
                                    }
                                    break;
                                default: //By default use a streamreader
                                    using (var sr2 = new StreamReader(fileStream))
                                        //use a streamreader to read the filestream
                                    {
                                        _response += sr2.ReadToEnd(); //add contents to the response string
                                        Log.Debug(sr2.ReadToEnd());
                                    }
                                    break;
                            }
                            break;
                        }
                        //If the file doesn't exist, return "404 Not Found"
                        Log.Info("Server response: Not Found");
                        //Create HTTP 404 header
                        _response += "HTTP/1.0 404 Not Found\r\n";
                        _response += "\r\n";
                        _response += "<html>404 Not Found</html>";
                        break;
                    }
                    Log.Info("Server response: Illegal request.");
                    //Create HTTP 400 header
                    _response += "HTTP/1.0 400 Illegal request\r\n";
                    _response += "\r\n";
                    _response += "<html>400 Illegal Request</html>";
                    break;
                }
            }
            try //Try to send the response
            {
                if (ar.Length != 0)
                {
                    sw.BaseStream.Write(ar, 0, ar.Length);
                }
                _response += "\r\n"; //End the response with an empty line
                sw.Write(_response); //write the response in the streamwriter
                sw.Flush(); //send the response
                Log.Info("Server response sent");
            }
            finally //Close the connection
            {
                Log.Info("Closed connection");
                ns.Close(); //Close network stream
                _connection.Close(); //Close connection socket
            }
        }
    }
}
