/* SQUARE-PIECE VALUES:
 *  0: empty
 *  1: pawn
 *  2: knight
 *  3: bishop
 *  4: rook
 *  5: queen
 *  6: king
 *  negative values represent black pieces, positive values represent white pieces. */

/* COLOR-REPRESENTATIONS:
 * -1: black
 *  1: white */

/* CASTLING RIGHTS:
 * Castling rights are represented as a tuple of four binary values, ordered as follows:
 *     (white's short castling rights,
 *      black's short castling rights,
 *      white's long castling rights,
 *      black's long castling rights) */

namespace Chess
{
    static class Constants
    {
        public const int BLACK = -1;
        public const int WHITE = 1;

        public const int EMPTY_SQUARE = 0;

        public const int WHITE_PAWN = 1;
        public const int WHITE_KNIGHT = 2;
        public const int WHITE_BISHOP = 3;
        public const int WHITE_ROOK = 4;
        public const int WHITE_QUEEN = 5;
        public const int WHITE_KING = 6;

        public const int BLACK_PAWN = -1;
        public const int BLACK_KNIGHT = -2;
        public const int BLACK_BISHOP = -3;
        public const int BLACK_ROOK = -4;
        public const int BLACK_QUEEN = -5;
        public const int BLACK_KING = -6;

        public const float WHITE_WIN = 1f;
        public const float BLACK_WIN = 0f;
        public const float DRAW = 0.5f;

        public static readonly int[,] STANDARD_POSITION = new int[8, 8] {
            {-4,-1,0,0,0,0,1,4},
            {-2,-1,0,0,0,0,1,2},
            {-3,-1,0,0,0,0,1,3},
            {-5,-1,0,0,0,0,1,5},
            {-6,-1,0,0,0,0,1,6},
            {-3,-1,0,0,0,0,1,3},
            {-2,-1,0,0,0,0,1,2},
            {-4,-1,0,0,0,0,1,4}
        };

        public static (int, int)[] MOVE_DIRECTIONS(int index)
        {
            switch (index)
            {
                case -6: return new(int, int)[] { (1, 1), (-1, 1), (-1, -1), (1, -1), (0, 1), (0, -1), (1, 0), (-1, 0), (2, 0), (-2, 0) };
                case -5: return new(int, int)[] { (1, 1), (-1, 1), (-1, -1), (1, -1), (0, 1), (0, -1), (1, 0), (-1, 0) };
                case -4: return new(int, int)[] { (0, 1), (0, -1), (1, 0), (-1, 0) };
                case -3: return new(int, int)[] { (1, 1), (-1, 1), (-1, -1), (1, -1) };
                case -2: return new(int, int)[] { (1, 2), (-1, 2), (1, -2), (-1, -2), (2, 1), (-2, 1), (2, -1), (-2, -1) };
                case -1: return new(int, int)[] { (0, 1), (1, 1), (-1, 1), (0, 2) };
                case 1: return new(int, int)[] { (0, -1), (1, -1), (-1, -1), (0, -2) };
                case 2: return new(int, int)[] { (1, 2), (-1, 2), (1, -2), (-1, -2), (2, 1), (-2, 1), (2, -1), (-2, -1) };
                case 3: return new(int, int)[] { (1, 1), (-1, 1), (-1, -1), (1, -1) };
                case 4: return new(int, int)[] { (0, 1), (0, -1), (1, 0), (-1, 0) };
                case 5: return new(int, int)[] { (1, 1), (-1, 1), (-1, -1), (1, -1), (0, 1), (0, -1), (1, 0), (-1, 0) };
                case 6: return new(int, int)[] { (1, 1), (-1, 1), (-1, -1), (1, -1), (0, 1), (0, -1), (1, 0), (-1, 0), (2, 0), (-2, 0) };
                default: return null;
            }
        }

        public static readonly bool[] STANDARD_CASTLING_RIGHTS = new bool[4] { true, true, true, true };
    }

}
