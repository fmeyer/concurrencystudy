using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Server.Service
{
    public partial class CalculatorService
    {
        private readonly IPAddress _ipAddress;
        private readonly int _port;
        private readonly IPEndPoint _localEndPoint;
        public static ManualResetEvent Mre = new ManualResetEvent(false);

        private Socket _listener;

        private const int MaxClients = 100;

        private readonly List<ClientSession> _clients = new List<ClientSession>();


        public CalculatorService(IPAddress ipAddress, int port)
        {
            _ipAddress = ipAddress;
            _port = port;

            _localEndPoint = new IPEndPoint(ipAddress, port);
        }
        public void Listen()
        {
            Logger.Info($"Server starting at {_ipAddress}:{_port}");
            
            _listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                _listener.Bind(_localEndPoint);
                _listener.Listen(MaxClients);

                while (true)
                {
                    // Blocking threads  
                    Mre.Reset();

                    Console.WriteLine("Waiting for a connection...");

                    // Start an asynchronous socket to listen for connections.
                    _listener.BeginAccept(AcceptCallback, _listener);

                    // Block and wait until a connection is established.  
                    Mre.WaitOne();
                }
            }
            catch (Exception e)
            {
                Logger.Error(e.ToString());
            }
        }

        // State object for reading client data asynchronously  

        private void AcceptCallback(IAsyncResult ar)
        {
            // Signal the main thread to continue.  
            Mre.Set();

            // Get the socket that handles the client request.
            _listener = (Socket)ar.AsyncState;
            var handler = _listener.EndAccept(ar);

            // Create the state object.  
            var client = new ClientSession(_clients.Count, handler);
            _clients.Add(client);

            Logger.Info($"Client ID: {client.ClientId} connected");
            handler.BeginReceive(client.Buffer, 0, ClientSession.BufferSize, 0, ReadCallback, client);
        }

        private void ReadCallback(IAsyncResult ar)
        {
            // Retrieve the state object and the handler socket  
            // from the asynchronous state object.  
            var client = (ClientSession)ar.AsyncState;
            var handler = client.WorkSocket;

            try
            {
                // Read data from the client socket.   
                var bytesRead = handler.EndReceive(ar);

                if (bytesRead <= 0) return;


                var s = Encoding.ASCII.GetString(client.Buffer, 0, bytesRead).Replace("\r", ""); // Telnet sends \r\n 
                var messages = s.Split("\n", StringSplitOptions.RemoveEmptyEntries);

                var list = new List<string>();
                list.AddRange(messages);

                foreach (var message in messages)
                {
                    
                    //  Graceful client shutdown request
                    if (message.IndexOf("done", StringComparison.Ordinal) > -1)
                    {
                        client.Shutdown();
                        _clients.Remove(client);
                        Logger.Info($"Client ID: {client.ClientId} disconnected");
                    }
                    else
                    {
                        client.ProcessMessage(message);
                        handler.BeginReceive(client.Buffer, 0, ClientSession.BufferSize, 0, ReadCallback, client);
                    }
                }                
            }
            catch (SocketException e)
            {
                _clients.Remove(client); // removing clients
                Logger.Error(e.ToString());
            }
        }
    }
}
