using System;
using System.IO;
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
        private static readonly ILog Log = LogManager.GetLogger(typeof(Client)); //Logging
        private string _response = ""; //Empty response string, is build upon in Run()
        private readonly Socket _connection; //The connection
        private const string RootCatalog = "c:/temp"; //Where to look for files
        private static readonly string[] Methods = {"GET", "POST"}; //A list of available methods
        private NetworkStream _ns; //The network stream

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
            //Set up streams
            _ns = new NetworkStream(_connection);
            var sr = new StreamReader(_ns);
            var sw = new StreamWriter(_ns);

            byte[] ar = {}; //An empty byte array, for picture requests

            var message = sr.ReadLine(); //Get a request
            Log.Info("Client (" + _connection.RemoteEndPoint + ") message: '" + message);
                
            //Is the request in the correct format?
            if (message != null && Regex.IsMatch(message, @"^[A-Z]{3,4} /.{0,150} HTTP/[0-9]\.[0-9]$"))
            {
                //Methods.Contains(request[0])
                string[] request = message.Split(' '); //The request string in parts, to check for file name and method name
                var filename = request[1]; //The 2nd element of the splitted request string, the filename

                if (request[2] == "HTTP/1.2") //The 3rd element, the protocol
                {
                    AddHeaders(400, "Illegal protocol"); //If the protocol is illegal
                }
                if (File.Exists(RootCatalog + request[1])) //Does the file exist?
                {
                    switch (request[0]) //Do different things for different methods
                    {
                        case "POST":
                            AddHeaders(200, "xxx"); //Not yet implemented header
                            break;
                        case "GET":
                            var fileStream = new FileStream(RootCatalog + filename, FileMode.Open, FileAccess.Read); //Initialize a filestream
                            //Create HTTP header
                            AddHeaders(200, "OK",
                                new[]{"Content-Type: " + ContentType.GetContentType(filename)
                                    ,"Content-Length: " + fileStream.Length
                                    ,"Date: " + DateTime.Now.Date.ToUniversalTime().ToString("r")});
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
                }
                else
                {
                    AddHeaders(404, "Not Found"); //If the file doesn't exist
                }
            }
            else
            {
                AddHeaders(400, "Illegal request"); //If the request doesn't match the format
            }

            try //Try to send the response
            {
                if (ar.Length != 0) sw.BaseStream.Write(ar, 0, ar.Length); //If the picture byte array is not empty, add it to the response stream
                sw.Write(_response); //write the response in the streamwriter
                sw.Flush(); //send the response
                Log.Info("Server response sent");
            }
            finally //Close the connection
            {
                Close();
            }
        }

        /// <summary>
        /// Adds headers to the response string.
        /// </summary>
        /// <param name="code">The status code</param>
        /// <param name="status">The status string</param>
        /// <param name="extraHeaders">Extra headers that should be added</param>
        public void AddHeaders(int code, string status, string[] extraHeaders = null)
        {
            Log.Info("Server response: " + code + " " + status + ".");
            _response += "HTTP/1.0 " + code + "" + status + "\r\n";

            if (extraHeaders != null)
                foreach (var header in extraHeaders)
                {
                    _response += header + "\r\n";
                }

            _response += "\r\n";
            if (code != 200)
            {
                _response += "<html>" + code + "" + status + "</html>";
            }
        }

        /// <summary>
        /// Closes the connection.
        /// </summary>
        public void Close()
        {
            Log.Info("Closed connection");
            _ns.Close(); //Close network stream
            _connection.Close(); //Close connection socket
        }
    }
}
