using System;
using Xunit;

using AddServer;

namespace AddServer.Test
{
    public class CalculatorTest
    {
        [Fact]
        public void TestAddingSingleNumbers()
        {
            Assert.True(AddServer.Calculator.Add(1, 2) == 3, "Should be equal to 3");
        }

        [Fact]
        public void TestAddingWhichOverFlowsResult()
        {
            Int32 op1 = Int32.MaxValue;
            Assert.Equal(op1 + op1, Calculator.Add(op1, op1));
        }

        [Fact]
        public void TestAddingHighandLowerBounds()
        {
            Int32 op1 = Int32.MaxValue; 
            Int32 op2 = Int32.MinValue;

            Assert.Equal(-1, Calculator.Add(op1, op2));
        }
    }
}
