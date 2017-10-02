using System;
using System.Net;
using Server.Service;
using Xunit;

namespace Client.Test
{
    public class IntegrationTest
    { 
        
        [Fact]
        public void Test1()
        {
            
            var port = new Random().Next(8000, 9000);
            
            new CalculatorService(IPAddress.Any, port).Listen();

            var client = new AddClient($"localhost:{port}");

            var result = client.Add(1, 2);
            
            Assert.Equal(3, result);
        }
    }
}
