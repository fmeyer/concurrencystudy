using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
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
        
        private static readonly ManualResetEvent ProcessDone = new ManualResetEvent(false);
        private static readonly ManualResetEvent ConnectDone = new ManualResetEvent(false);
        private static readonly ManualResetEvent SendDone = new ManualResetEvent(false);

        // A thread-safe FIFO which receives in order the server responses
        private readonly ConcurrentQueue<string> _responses;


        private readonly Socket _client; 

        public AddClient(IPAddress iP, int port)
        {

            var remoteEp = new IPEndPoint(iP, port);

            // Create a TCP/IP socket.  
            _client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            // Connect to the remote endpoint.  
            _client.BeginConnect(remoteEp, ConnectCallback, _client);
            ConnectDone.WaitOne();

            _responses = new ConcurrentQueue<string>();
        }
        
        /**
         * Syncronous method
         */
        public int Add(int p0, int p1)
        {
            var request = new SumRequest(p0, p1);
            
            Send(request.ToString());
            return int.Parse(Receive());
        }

        /**
         * Assyncronous method
         */
        public async Task<int> AddAsync(int p0, int p1)
        {

            var request = new SumRequest(p0, p1);

            ProcessDone.Reset();

            // Send test data to the remote device.  
            SendAsync(_client, request.ToString());

            // TODO: split send and receive
            ReceiveAsync(_client);
            ProcessDone.WaitOne();

            _responses.TryDequeue(out var response);

            // Receive the response from the remote device.      
            var r = int.Parse(response);

            return await Task.FromResult(r); // wrapping response into an Task to enable the await keyword
        }

        public void Dispose(){
            Close();
        }

        public void Close() {
            Disconnect();
        }

        private void Disconnect()
        {

            // Release the socket.  
            Send("done");
            Receive();
            _client.Close();

            Console.WriteLine("Disconnect from server");
        }
    }
}