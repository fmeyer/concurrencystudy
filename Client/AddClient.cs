using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Client
{
    public class AddClient
    {
        public AddClient(string localhost)
        {
            TcpClient clientSocket = new TcpClient();
            
            clientSocket.Connect("127.0.0.1", 1233);
            
            NetworkStream serverStream = clientSocket.GetStream();
            
            byte[] outStream = System.Text.Encoding.ASCII.GetBytes("21\n12\n21\n12\n21\n12\n");
            serverStream.Write(outStream, 0, outStream.Length);
            
            serverStream.Flush();

            outStream = System.Text.Encoding.ASCII.GetBytes("43\n");

            serverStream.WriteAsync(outStream, 0, outStream.Length, CancellationToken.None);

            byte[] inStream = new byte[32];
            
            serverStream
            
            serverStream.Read(inStream, 0, clientSocket.ReceiveBufferSize);
            var returndata = System.Text.Encoding.ASCII.GetString(inStream);

            Console.WriteLine(returndata);
        }

        public async Task<int> AddAsync(int p0, int p1)
        {
            
            // spawn operation which waits for return; 
            // assume response cames in order;
            // max server buffer is 1024;
            
            
            return 0;
        }

        ~AddClient()
        {
            disconnect();
            
        }

        private void disconnect()
        {
            Console.WriteLine("Disconnect from server");
        }
    }
}