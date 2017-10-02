using System;
using System.Net;
using System.Threading;

namespace Client
{
    class Client
    {

        private async void RequestAsync()
        {
            var client = new AddClient(IPAddress.Any, 1133);
            var first = await client.AddAsync(10, 20);
            var second = await client.AddAsync(30, 40);

            Console.WriteLine ($"first {first} second {second}");
        }
        
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            new Client().RequestAsync();
        }
    }
}
