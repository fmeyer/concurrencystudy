namespace Client
{
    public partial class AddClient
    {
        public class SumRequest
        {

            private readonly  int _op1;
            private readonly  int _op2;

            public override string ToString()
            {
                return $"{_op1}\n{_op2}\n"; // need to send two separated messages
            }

            public SumRequest(int op1 = 0, int op2 = 0)
            {
                _op1 = op1;
                _op2 = op2;
            }
        }
    }
}