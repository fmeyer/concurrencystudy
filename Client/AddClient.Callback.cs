using System;
using System.Net.Sockets;
using System.Text;

namespace Client
{
    public partial class AddClient
    {
        private void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                var state = (ResultBuffer) ar.AsyncState;
                var client = state.ClientSocket;
  
                var bytesRead = client.EndReceive(ar);

                if (bytesRead <= 0) return;

                var result = Encoding.ASCII.GetString(state.Buffer, 0, bytesRead);
                
                _responses.Enqueue(result);

                // signal workers;
                processDone.Set();
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
                // Console.WriteLine("Sent {0} bytes to server.", bytesSent);
  
                sendDone.Set();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}