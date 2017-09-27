using Xunit;

namespace Server.Test
{
    public class CalculatorTest
    {
        [Fact]
        public void TestAddingSingleNumbers()
        {
            Assert.True(Service.Command.Calculator.Add(1, 2) == 3, "Should be equal to 3");
        }

        [Fact]
        public void TestAddingWhichOverFlowsResult()
        {
            const int op1 = int.MaxValue;
            
            Assert.Equal(unchecked(op1 + op1), Service.Command.Calculator.Add(op1, op1));
        }

        [Fact]
        public void TestAddingHighandLowerBounds()
        {
            const int op1 = int.MaxValue; 
            const int op2 = int.MinValue;

            Assert.Equal(-1, Service.Command.Calculator.Add(op1, op2));
        }
    }
}
