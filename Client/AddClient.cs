using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Client
{
    public partial class AddClient
    {
        // ManualResetEvent instances signal completion.  
        
        private static readonly ManualResetEvent processDone = new ManualResetEvent(false);
        private static readonly ManualResetEvent connectDone = new ManualResetEvent(false);
        private static readonly ManualResetEvent sendDone = new ManualResetEvent(false);

        private readonly AddRequestQueue _requests;
        private readonly Queue<string> _results;

        private readonly Socket _client; 

        public AddClient(string localhost)
        {
            var strings = localhost.Split(":");

            // var ipHostInfo = Dns.Resolve(strings[0]);
            var remoteEP = new IPEndPoint(IPAddress.Any, int.Parse(strings[1]));

            // Create a TCP/IP socket.  
            _client = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);

            // Connect to the remote endpoint.  
            _client.BeginConnect(remoteEP, ConnectCallback, _client);
            connectDone.WaitOne();


            _requests = new AddRequestQueue(10); // 10 workers
            _results = new Queue<string>();
        }

        private void Receive(Socket client)
        {
            var state = new ResultBuffer();
            
            try
            {
                // Create the state object.  
                state.ClientSocket = client;

                // Begin receiving the data from the remote device.  
                client.BeginReceive(state.Buffer, 0, ResultBuffer.BufferSize, 0, ReceiveCallback, state);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private void Send(Socket client, string data)
        {
  
            var byteData = Encoding.ASCII.GetBytes(data);

            client.BeginSend(byteData, 0, byteData.Length, 0, SendCallback, client);
        }

        public async Task<int> AddAsync(int p0, int p1)
        {
            var request = new SumRequest(p0, p1);

            processDone.Reset();

            _requests.Enqueue(() =>
            {
                // Send test data to the remote device.  
                Send(_client, request.ToString());
                sendDone.WaitOne();
                
                Receive(_client);
            });

            processDone.WaitOne();
            
            // Receive the response from the remote device.      
            var r = int.Parse(_results.Dequeue());
            return await Task.FromResult(r);
        }

        public void Close()
        {
            _requests.Dispose();
        }

        ~AddClient()
        {
            Disconnect();
        }

        private void Disconnect()
        {
                        
            // Release the socket.  
            _client.Shutdown(SocketShutdown.Both);
            _client.Close();

            Console.WriteLine("Disconnect from server");
        }

        private class ResultBuffer
        {
            // Size of receive buffer.  
            public const int BufferSize = 1024;

            // Receive buffer.  
            public readonly byte[] Buffer = new byte[BufferSize];

            // Client socket.  
            public Socket ClientSocket;
        }
    }
}