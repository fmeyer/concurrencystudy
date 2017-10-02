using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Server.Service;
using Xunit;

namespace Client.Test
{
    public class IntegrationTest
    {

        int port = new Random().Next(8000, 9000);
        CalculatorService calc;
        private readonly Task t;
        private AddClient client;

        public IntegrationTest()
        {

            t = Task.Run(() =>
            {
                calc = new CalculatorService(IPAddress.Any, port);
                calc.Listen();
            });
            
        }


        public void Dispose()
        {
            client.Dispose();
            t.Dispose();
        }

        [Fact]
        public void TestAddSync()
        {
            client = new AddClient(IPAddress.Any, port);

            var result = client.Add(1, 2);
            Assert.Equal(3, result);

        }

        /**
            The simple async testcase where order is maintained and the method
            awaits the result before moving further.
         */
        [Fact]
        public async void TestAddAsync() {

            client = new AddClient(IPAddress.Any, port);

            var first = await client.AddAsync(10, 20);
            var second = await client.AddAsync(30, 40);


            Assert.Equal(30, first);
            Assert.Equal(70, second);
        }

        /** 
            This test sends several messages to the server and waits for the result in order
            The test is failing intermitently, since I need to figure out a way of maintaing 
            order without using a sequence id on the message;
         */
        [Fact]
        public void TestBurstAsync()
        {

            var MaxThreads = 10;
            Thread[] _workers = new Thread[MaxThreads];

            client = new AddClient(IPAddress.Any, port);

            for (int i = 0; i < MaxThreads; i++)
            {
                var i1 = i; // need to pass a copy !?

                (_workers[i] = new Thread((async () =>
                {
                    Thread.Sleep(new Random().Next(200));
                    var result = await client.AddAsync(i1, 1);
                    Console.WriteLine($"adding {i1} and {1} results {result}");
                    Assert.Equal(i1 + 1, result);                    
                }))).Start();
            }

            foreach (var worker in _workers) worker.Join();
        }
    }
}
