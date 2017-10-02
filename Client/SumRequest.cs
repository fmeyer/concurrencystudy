namespace Client
{
    public partial class AddClient
    {
        public class SumRequest
        {
            
            public enum States {Pending, Processing, Done};  

            private readonly  int _op1;
            private readonly  int _op2;

            public int Result { get; set; }

            public States State { get; set; }

            public override string ToString()
            {
                return $"{_op1}\n{_op2}\n"; // need to send two 
            }

            public SumRequest(int op1 = 0, int op2 = 0)
            {
                _op1 = op1;
                _op2 = op2;

                State = States.Pending;
            }
        }
    }
}