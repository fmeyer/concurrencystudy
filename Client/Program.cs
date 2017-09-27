using System;
using System.Threading;

namespace Client
{
    class Program
    {
        private async void RequestAsync()
        {
            var client = new AddClient("localhost:1233");
            var first = await client.AddAsync(10, 20);
            var second = await client.AddAsync(30, 40);

            for (int i = 0; i < 10000; i++)
            {
                Thread t;

                var i1 = i;
                t = new Thread((async () =>
                {
                    var result = await client.AddAsync(i1, 1);
                    Console.WriteLine($"adding {i1} and {1} results {result}");
                }));
                
                t.Start();
                t.Join();
            }

            Console.WriteLine ($"first {first} second {second}");  // Needs to print: first
            
            client.Close();
        }
        
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            new Program().RequestAsync();
        }
    }
}
