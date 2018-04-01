using System;
using System.Collections.Generic;
using System.Linq;

const sbyte[,] STANDARD_POSITION = new int[8,8] {
  {-4,-1,0,0,0,0,1,4},
  {-2,-1,0,0,0,0,1,2},
  {-3,-1,0,0,0,0,1,3},
  {-5,-1,0,0,0,0,1,5},
  {-6,-1,0,0,0,0,1,6},
  {-3,-1,0,0,0,0,1,3},
  {-2,-1,0,0,0,0,1,2},
  {-4,-1,0,0,0,0,1,4}
};

public static Point[] MOVE_DIRECTIONS(sbyte index) {
  switch (index) {
    case -6: return new Point[] { new Point { 1, 1 }, new Point { -1,1 }, new Point {-1,-1 }, new Point { 1,-1 }, new Point { 0, 1 }, new Point { 0,-1 }, new Point { 1, 0 }, new Point { -1,0 }, new Point { 2, 0 }, new Point { -2,0 } };
    case -5: return new Point[] { new Point { 1, 1 }, new Point { -1,1 }, new Point {-1,-1 }, new Point { 1,-1 }, new Point { 0, 1 }, new Point { 0,-1 }, new Point { 1, 0 }, new Point { -1,0 } };
    case -4: return new Point[] { new Point { 0, 1 }, new Point { 0,-1 }, new Point { 1, 0 }, new Point { -1,0 } };
    case -3: return new Point[] { new Point { 1, 1 }, new Point { -1,1 }, new Point {-1,-1 }, new Point { 1,-1 } };
    case -2: return new Point[] { new Point { 1, 2 }, new Point { -1,2 }, new Point { 1,-2 }, new Point {-1,-2 } };
    case -1: return new Point[] { new Point { 0, 1 }, new Point { 1, 1 }, new Point { -1,1 }, new Point { 0, 2 } };
    case  1: return new Point[] { new Point { 0,-1 }, new Point { 1,-1 }, new Point {-1,-1 }, new Point { 0,-2 } };
    case  2: return new Point[] { new Point { 1, 2 }, new Point { -1,2 }, new Point { 1,-2 }, new Point {-1,-2 } };
    case  3: return new Point[] { new Point { 1, 1 }, new Point { -1,1 }, new Point {-1,-1 }, new Point { 1,-1 } };
    case  4: return new Point[] { new Point { 0, 1 }, new Point { 0,-1 }, new Point { 1, 0 }, new Point { -1,0 } };
    case  5: return new Point[] { new Point { 1, 1 }, new Point { -1,1 }, new Point {-1,-1 }, new Point { 1,-1 }, new Point { 0, 1 }, new Point { 0,-1 }, new Point { 1, 0 }, new Point { -1,0 } };
    case  6: return new Point[] { new Point { 1, 1 }, new Point { -1,1 }, new Point {-1,-1 }, new Point { 1,-1 }, new Point { 0, 1 }, new Point { 0,-1 }, new Point { 1, 0 }, new Point { -1,0 }, new Point { 2, 0 }, new Point { -2,0 } };
  }
}

const bool[] STANDARD_CASTLING_RIGHTS = new bool[4] {true};

bool _kingInPosition(ref Position p)
{
    return Array.Exists(p.board, element => element == 6 || element = -6);
}

public class Position {
	public int toMove;
	public int fiftyMoveProximity;
	public int[,] board;
	public bool[] castlingRights;
	
	public Position(int[,] position = STANDARD_POSITION, int toMove = 1, int fiftyMoveProximity = 0, bool[] castlingRights = STANDARD_CASTLING_RIGHTS) {
		board = position;
		toMove = toMove;
		fiftyMoveProximity = fiftyMoveProximity;
		castlingRights = castlingRights;
	}

    public float findBestMove(int depth, float alpha, float beta) {
        float bestEvaluation = new float(2 * toMove);

        if (toMove == -1)
        {
            foreach (var position in generatePositions()) {
                if (!_kingInPosition(position))
                {
                    /* make sure that this move is never picked by the engine */
                }
                if (depth > 1)
                {
                    float evaluation = position.findBestMove(depth - 1, alpha, beta);
                }
                else
                {
                    float evaluation = Random.nextFloat(); /* implement evaluation network here, for now simply random value */ 
                }
            }
        }
    }
	
}

namespace Position
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("DORK");
            Console.ReadKey();
        }
    }
}
