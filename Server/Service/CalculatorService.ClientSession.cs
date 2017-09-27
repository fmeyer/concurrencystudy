using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

using Server.Service.Command;

namespace Server.Service
{
    public partial class CalculatorService
    {
        private class ClientSession
        {
            public readonly int ClientId;
            // Client  socket.  
            public readonly Socket WorkSocket;
            // Size of receive buffer.  
            public const int BufferSize = 32;
            // Receive buffer.  
            public readonly byte[] Buffer = new byte[BufferSize];

            private readonly Stack<int> _intBuffer = new Stack<int>();

            public ClientSession(int id, Socket handler){
                ClientId = id;
                WorkSocket = handler;
            }

            public void Shutdown()
            {
                Notify("See ya\n");
                WorkSocket.Shutdown(SocketShutdown.Both);
                WorkSocket.Close();
            }
            
            public void ProcessMessage(string message)
            {
                var data = 0;

                try
                {
                    data = int.Parse(message);
                    _intBuffer.Push(data);
                    PerformCommand(); // FIXME: Still doesn't support pipelining
                }
                catch (OverflowException)
                {
                    Logger.Error($"int32 overflow {message}");
                }
                catch (FormatException)
                {
                    Logger.Error($"Content not encoded in I32 {message}");
                }
            }

            private void PerformCommand()
            {
                
                if ((_intBuffer.Count <= 0) || (_intBuffer.Count % 2 != 0)) return;

                var op1 = _intBuffer.Pop();
                var op2 = _intBuffer.Pop();

                Notify($"{Calculator.Add(op1, op2)}\n");
            }


            private void Notify(string message)
            {
                // Convert the string data to byte data using ASCII encoding.  
                var byteData = Encoding.ASCII.GetBytes(message);

                // Begin sending the data to the remote device.  
                WorkSocket.BeginSend(byteData, 0, byteData.Length, 0, SendCallback, WorkSocket);             
            }
            
            private void SendCallback(IAsyncResult ar)
            {
                try
                {
                    // Retrieve the socket from the state object.  
                    var handler = (Socket)ar.AsyncState;

                    // Complete sending the data to the remote device.  
                    var bytesSent = handler.EndSend(ar);
                    Logger.Info($"Sent {bytesSent} bytes to client: {ClientId}");
                }
                catch (Exception e)
                {
                    Logger.Error(e.ToString());
                }
            }

        }
    }
}