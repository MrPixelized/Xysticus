using Chess;

namespace Interface
{
    public class FENParser
    {
        public static int[,] ParseFEN()
        {
            return Chess.Constants.STANDARD_POSITION;
        }
    }
}