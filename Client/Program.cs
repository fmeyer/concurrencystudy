using System;
using System.Threading;

namespace Client
{
    class Program
    {

        Thread[] _workers = new Thread[Program.MaxThreads];

        public static readonly int MaxThreads = 10;
        private async void RequestAsync()
        {
            var client = new AddClient("localhost:1233");
            var first = await client.AddAsync(10, 20);
            var second = await client.AddAsync(30, 40);

            for (int i = 0; i < Program.MaxThreads; i++)
            {
                var i1 = i;

                (_workers[i] = new Thread((async () =>
                {
                    Thread.Sleep(new Random().Next(200));
                    var result = await client.AddAsync(i1, 1);
                    Console.WriteLine($"adding {i1} and {1} results {result}");
                }))).Start();
            }

            Console.WriteLine ($"first {first} second {second}");  // Needs to print: first

            foreach (var worker in _workers) worker.Join();
            
            client.Close();
        }
        
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            new Program().RequestAsync();
        }
    }
}
