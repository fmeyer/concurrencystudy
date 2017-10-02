using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace Client
{
    public partial class AddClient
    {
        
        
        private string Receive()
        {
            ResultBuffer r = new ResultBuffer();
            var bytesRead = _client.Receive(r.Buffer, 0, ResultBuffer.BufferSize, SocketFlags.None);
            var result = Encoding.ASCII.GetString(r.Buffer, 0, bytesRead);
            return result;
        }

        private void Send(string request)
        {
            _client.Send(Encoding.ASCII.GetBytes(request));
        }
        
        
        private void ReceiveAsync(Socket client)
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
        
        private void SendAsync(Socket client, string data)
        {
  
            var byteData = Encoding.ASCII.GetBytes(data);

            client.BeginSend(byteData, 0, byteData.Length, 0, SendCallback, client);
        }
   
        
        private void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                var state = (ResultBuffer) ar.AsyncState;
                var client = state.ClientSocket;

  
                var bytesRead = client.EndReceive(ar);

                if (bytesRead <= 0)
                {
                    client.BeginReceive(state.Buffer, 0, ResultBuffer.BufferSize, 0, ReceiveCallback, state);
                }

                var result = Encoding.ASCII.GetString(state.Buffer, 0, bytesRead);

                // receive pipelined responses
                var messages = result.Split("\n", StringSplitOptions.RemoveEmptyEntries);

                var list = new List<string>();

                list.AddRange(messages);

                foreach (var message in messages)
                {
                    _responses.Enqueue(message);
                }
                processDone.Set(); // notify the waiting threads only if all responses arrived

                // if (received == sent) {
                // } else { 
                //     client.BeginReceive(state.Buffer, 0, ResultBuffer.BufferSize, 0, ReceiveCallback, state);
                // }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static void ConnectCallback(IAsyncResult ar)
        {
            try
            {  
                var client = (Socket) ar.AsyncState;


                client.EndConnect(ar);

                Console.WriteLine("Socket connected to {0}", client.RemoteEndPoint.ToString());
  
                // signal 
                connectDone.Set();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static void SendCallback(IAsyncResult ar)
        {
            try
            {  
                var client = (Socket) ar.AsyncState;
                var bytesSent = client.EndSend(ar);
  
                sendDone.Set();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
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