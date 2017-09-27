using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace addserver
{
    class Program
    {
        static void Main(string[] args)
        {
            int port = 1234;

            IPAddress ipAddress = IPAddress.Any; 
            Console.WriteLine($"Server starting at {ipAddress}:{port}");
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, port);
            Console.WriteLine("see ya");
        }
    }
}

