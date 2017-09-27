using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections;

namespace addserver
{

    class Calculator
    {
        public static Int32 Add(Int32 op1, Int32 op2)
        {
            return op1 + op2;
        }
    }
    class Program
    {

        static void Main(string[] args)
        {
            // FIXME: port as param 
            int port = 1233;

            byte[] buffer = new Byte[1024];

            IPAddress ipAddress = IPAddress.Any; // FIXME: ip addr or hostname as param
            Console.WriteLine($"Server starting at {ipAddress}:{port}");
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, port);
            Console.WriteLine($"see ya {Calculator.Add(1, 1)}");

            Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);


            listener.Bind(localEndPoint);
            listener.Listen(10);

            while (true)
            {
                System.Console.WriteLine("Waiting for connection");

                Socket connectionHandler = listener.Accept();
                String data = null;
                Int32 di32 = 0;

                Stack intBuffer = new Stack();

                while (true)
                {
                    buffer = new byte[1024];
                    int bytesReceived = connectionHandler.Receive(buffer);

                    Console.WriteLine($"received {bytesReceived}");

                    data = Encoding.ASCII.GetString(buffer, 0, bytesReceived);

                    if (data.IndexOf("done") > -1) // Graceful client shutdown request
                    {
                        break;
                    }

                    try
                    {
                        di32 = Int32.Parse(data);
                        intBuffer.Push(di32);
                    }
                    catch (System.OverflowException)
                    {
                        System.Console.WriteLine($"I32 overflow  {data}");
                        // error handling?
                    }
                    catch (System.FormatException)
                    {
                        System.Console.WriteLine($"not encoded in I32 {data}");
                    }

                    // process buffer
                    if (intBuffer.Count % 2 == 0)
                    {
                        Int32 op1 = (Int32)intBuffer.Pop();
                        Int32 op2 = (Int32)intBuffer.Pop();

                        byte[] result = Encoding.ASCII.GetBytes($"{Calculator.Add(op1, op2)}\n");

                        connectionHandler.Send(result);
                    }
                    Console.WriteLine($"parsed integer value {di32}");
                }

                // Graceful shutdown message
                byte[] msg = Encoding.ASCII.GetBytes("Bye!\n");

                connectionHandler.Send(msg);
                connectionHandler.Shutdown(SocketShutdown.Both);
                connectionHandler.Close();
            }
        }
    }
}
}
