namespace Chess
{
    static class Constants
    {
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
                case -2: return new(int, int)[] { (1, 2), (-1, 2), (1, -2), (-1, -2) };
                case -1: return new(int, int)[] { (0, 1), (1, 1), (-1, 1), (0, 2) };
                case 1: return new(int, int)[] { (0, -1), (1, -1), (-1, -1), (0, -2) };
                case 2: return new(int, int)[] { (1, 2), (-1, 2), (1, -2), (-1, -2) };
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
