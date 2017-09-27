namespace Server.Service.Command
{
    public class Calculator
    {
        /* A simple static calculator method that adds two operands and returns a value on the same domain
        I'm not handling buffer overflows */
        public static int Add(int op1, int op2)
        {
            return op1 + op2;
        }
    }
}
