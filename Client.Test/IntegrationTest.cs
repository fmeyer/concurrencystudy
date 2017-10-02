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
        private const int MaxThreads = 10;

        private readonly int _port = new Random().Next(8000, 9000);
        private readonly Task _t;
        private AddClient _client;

        public IntegrationTest()
        {
            _t = Task.Run(() =>
            {
                var calc = new CalculatorService(IPAddress.Any, _port);
                calc.Listen();
            });
        }

        public void Dispose()
        {
            _client.Dispose();
            _t.Dispose();
        }

        [Fact]
        public void TestAddSync()
        {
            _client = new AddClient(IPAddress.Any, _port);

            var result = _client.Add(1, 2);
            Assert.Equal(3, result);
        }

        /**
            The simple async testcase where order is maintained and the method
            awaits the result before moving further.
         */
        [Fact]
        public async void TestAddAsync() {

            _client = new AddClient(IPAddress.Any, _port);

            var first = await _client.AddAsync(10, 20);
            var second = await _client.AddAsync(30, 40);


            Assert.Equal(30, first);
            Assert.Equal(70, second);
        }


        [Fact]
        public async void TestAddAsyncTwoClients()
        {
            
        AddClient _client1 = new AddClient(IPAddress.Any, _port);
        AddClient _client2 = new AddClient(IPAddress.Any, _port);

        var first = await _client1.AddAsync(10, 20);
        var second = await _client2.AddAsync(30, 40);

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
            Thread[] workers = new Thread[MaxThreads];

            _client = new AddClient(IPAddress.Any, _port);

            for (int i = 0; i < MaxThreads; i++)
            {
                var i1 = i; // need to pass a copy !?

                (workers[i] = new Thread((async () =>
                {
                    Thread.Sleep(new Random().Next(200));
                    var result = await _client.AddAsync(i1, 1);
                    Console.WriteLine($"adding {i1} and {1} results {result}");
                    Assert.Equal(i1 + 1, result);                    
                }))).Start();
            }

            foreach (var worker in workers) worker.Join();
        }
    }
}
