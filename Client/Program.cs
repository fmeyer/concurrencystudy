using System;

namespace Client
{
    class Program
    {
        private async void Request()
        {
            var client = new AddClient("localhost:1234");
            var first = await client.AddAsync(10, 20);
            var second = await client.AddAsync(30, 40);
            
            Console.WriteLine ($"first {first} second {second}");  // Needs to print: first             
        }
        
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            new Program().Request();
        }
    }
}
