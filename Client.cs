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
        private static readonly ILog log = LogManager.GetLogger(typeof(Client));
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

            var readFirstLine = true; //initialize
            var requestStr = "";
            var request = new[] { "" }; //initialize
            while (true)
            {
                var message = sr.ReadLine(); //Get a request
                if (message != null)
                {
                    log.Info("Client (" + _connection.RemoteEndPoint + ") message: '" + message);
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
                    //Is the request in the correct format?
                    if (Regex.IsMatch(requestStr, @"^[A-Z]{3,4} /.{0,150} HTTP/[0-9]\.[0-9]$") && Methods.Contains(request[0]))
                    {
                        //Is it an illegal protocol?
                        if (request[2] == "HTTP/1.2")
                        {
                            log.Info("Server response: Illegal protocol");
                            //Create HTTP 404 header
                            _response += "HTTP/1.0 400 Illegal protocol\r\n";
                            _response += "\r\n";
                            _response += "<html>400 Illegal protocol</html>";
                            break;
                        }
                        //Does the file exist?
                        if (File.Exists(RootCatalog + request[1]))
                        {
                            //Is it post?
                            if (request[0] == "POST")
                            {
                                //Create HTTP not yet implemented header
                                _response += "HTTP/1.0 200 xxx\r\n";
                                _response += "Content-Type: text/html\r\n";
                                _response += "\r\n";
                                break;
                            }
                            //Create HTTP header
                            _response += "HTTP/1.0 200 OK\r\n";
                            _response += "Content-Type: text/html\r\n";
                            _response += "\r\n";
                            //Create body from the specified file
                            var fileStream = new FileStream(RootCatalog + request[1], FileMode.Open,
                                FileAccess.Read);
                            //New filestream
                            using (var sr2 = new StreamReader(fileStream)) //use a streamreader to read the filestream
                            {
                                _response += sr2.ReadToEnd(); //add contents to the response string
                            }
                            break;
                        }
                        //If the file doesn't exist, return "404 Not Found"
                        log.Info("Server response: Not Found");
                        //Create HTTP 404 header
                        _response += "HTTP/1.0 404 Not Found\r\n";
                        _response += "\r\n";
                        _response += "<html>404 Not Found</html>";
                        break;
                    }
                    log.Info("Server response: Illegal request.");
                    //Create HTTP 400 header
                    _response += "HTTP/1.0 400 Illegal request\r\n";
                    _response += "\r\n";
                    _response += "<html>400 Illegal Request</html>";
                    break;
                }
            }
            try
            {
                sw.Write(_response); //write the response in the streamwriter
                sw.Flush(); //send the response
                log.Info("Server response sent");
            }
            finally
            {
                ns.Close();
                log.Info("Closed connection");
            }
        }
    }
}
