/* MOVE:
 * Move is simply a more convenient placeholder for a ValueTuple`4 */

namespace Chess
{
    public struct Move
    {
        public int fromX;
        public int fromY;
        public int toX;
        public int toY;

        public Move(int x1, int y1, int x2, int y2)
        {
            fromX = x1;
            fromY = y1;
            toX = x2;
            toY = y2;
        }
    }

}
