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
        private static readonly ManualResetEvent Mre = new ManualResetEvent(false);

        private TcpListener _listener;

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

            _listener = new TcpListener(_localEndPoint);
            _listener.Start();

            while (true)
            {                
                // Blocking threads  
                Mre.Reset();

                Console.WriteLine("Waiting for a connection...");

                // Start an asynchronous socket to listen for connections.
                _listener.BeginAcceptSocket(AcceptCallback, _listener);

                // Block and wait until a connection is established.  
                Mre.WaitOne();
            }
        }

        // State object for reading client data asynchronously  

        private void AcceptCallback(IAsyncResult ar)
        {
            // Signal the main thread to continue.  
            Mre.Set();

            // Get the socket that handles the client request.
            _listener = (TcpListener) ar.AsyncState;
            var handler = _listener.EndAcceptSocket(ar);

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
