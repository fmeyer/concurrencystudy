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
        
        private static readonly ManualResetEvent processDone = new ManualResetEvent(false);
        private static readonly ManualResetEvent connectDone = new ManualResetEvent(false);
        private static readonly ManualResetEvent sendDone = new ManualResetEvent(false);


        private readonly ConcurrentQueue<SumRequest> _requests;
        private readonly ConcurrentQueue<string> _responses;


        private readonly Socket _client; 

        public AddClient(IPAddress iP, int port)
        {

            // var ipHostInfo = Dns.Resolve(strings[0]);
            var remoteEP = new IPEndPoint(iP, port);

            // Create a TCP/IP socket.  
            _client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            // Connect to the remote endpoint.  
            _client.BeginConnect(remoteEP, ConnectCallback, _client);
            connectDone.WaitOne();

            _responses = new ConcurrentQueue<string>();
            _requests = new ConcurrentQueue<SumRequest>();
            
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
            var result = 0;
            
            var thread = new Thread(start: (async () =>
            {
                result = Add(p0, p1);
            }));
            
            thread.Start();

            thread.Join();

            return await Task.FromResult(result);
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