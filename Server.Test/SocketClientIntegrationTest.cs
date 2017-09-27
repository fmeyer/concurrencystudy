using System;

using System.Net;
using System.Net.Sockets;

using Xunit;
using Server;

namespace Server.Test
{
    public class SocketClientIntegrationTest
    {
        [Fact]
        public void TestSimpleConnection()
        {

            // int randomPort = new Random().Next(40000, 41000);
            
            // CalculatorService calculator = new CalculatorService(IPAddress.Any, randomPort);

            // calculator.Listen();

            // Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            // System.Net.IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, randomPort);
            // socket.Connect(remoteEP);


            // //Start sending stuf..
            // byte[] byData = System.Text.Encoding.ASCII.GetBytes("done");
            // socket.Send(byData);


            // byte[] buffer = new byte[1024];
            // int iRx = socket.Receive(buffer);
            // char[] chars = new char[iRx];

            // System.Text.Decoder d = System.Text.Encoding.UTF8.GetDecoder();
            // int charLen = d.GetChars(buffer, 0, iRx, chars, 0);
            // System.String recv = new System.String(chars);

            // Assert.Equal("bye", recv);

            // calculator.Halt();
            
        }
    }

}
