using System;
using Xunit;

using AddServer;

namespace AddServer.Test
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            
            Assert.False(AddServer.Calculator.Add(1, 2) == 3, "Should not be equal to 3");
        }
    }
}
