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

struct Move
{
    int fromX;
    int fromY;
    int toX;
    int toY;

    public Move(int x1, int y1, int x2, int y2)
    {
        fromX = x1;
        fromY = y1;
        toX = x2;
        toY = y2;
    }
}

public class Position {
	public int toMove;
	public int fiftyMoveProximity;
	public int[,] board;
	public bool[] castlingRights;
	
	public Position(int[,] board = STANDARD_POSITION, int toMove = 1, int fiftyMoveProximity = 0, bool[] castlingRights = STANDARD_CASTLING_RIGHTS) {
		this.board = board;
		this.toMove = toMove;
		this.fiftyMoveProximity = fiftyMoveProximity;
		this.castlingRights = castlingRights;
	}

    public float findBestMove(int depth, float alpha, float beta) {
        float bestEvaluation = new float(2 * toMove);

        if (toMove == -1)
        {
            foreach ((position, move) in generatePositions()) {
                if (!position._kingsInPosition())
                {
                    continue;
                }
                if (depth > 1)
                {
                    float evaluation = position.findBestMove(depth - 1, alpha, beta);
                }
                else
                {
                    float evaluation = Random.nextFloat(); /* implement evaluation network here, for now simply random value */ 
                }
                bestEvaluation = Math.Max(bestEvaluation, evaluation);
                alpha = Math.Max(bestEvaluation, beta);
                if (beta <= alpha)
                {
                    break;
                }
            }
        }

        else
        {
            foreach ((position, move) in generatePositions()) {
                if (!position._kingsInPosition())
                {
                    continue;
                }
                if (depth > 1)
                {
                    float evaluation = position.findBestMove(depth - 1, alpha, beta);
                }
                else
                {
                    float evaluation = Random.nextFloat(); /* implement eveluation network here, for now simply random value */
                }
                bestEvaluation = Math.Min(bestEvaluation, evaluation);
                beta = Math.Min(bestEvaluation, beta);
                if (beta <= alpha) {
                    break;
                }
            }
        }

        return bestEvaluation;
    }

    public IEnumerable<Position, Move> generatePositions()
    {
        foreach ((squareContent, x, y) in _iteratePosition())
        {
            if (_isPiece(squareContent) && squareContent * toMove > 0)
            {
                foreach (Point move in MOVE_DIRECTIONS(squareContent))
                {
                    for (int m = 1; m < 9; m++)
                    {
                        (toX, toY) = (x + move.X * m, y + move.Y * m);
                        if (!legalMove(x, y, toX, toY))
                        {
                            break;
                        }
                        else
                        {
                            yield return (makeMove(x, y, toX, toY), new Move (x, y, toX, toY));
                        }
                        if (_isKing(squareContent) || isKnight(squareContent) || isPawn(squareContent))
                        {
                            break;
                        }
                        if (_isPiece(board[x + toX][y + toY]))
                        {
                            break;
                        }
                    }
                }
            }
        }
    }

    private bool _legalMove(int x, int y, int toX, int toY) => true;

    private bool _kingsInPosition
    {
        get
        {
            int kingCount = 0;
            for (int x; x < 8; x++)
            {
                for (int y; y < 8; y++)
                {
                    switch (board[x][y])
                    {
                        case -6: kingCount++;
                        case 6: kingCount++;
                    }
                }
            }

            return (kingCount == 2);
        }
    }

    private IEnumerable<(int, int, int)> _iteratePosition()
    {
        for (int x; x < 8; x++)
        {
            for (int y; y < 8; y++)
            {
                yield return (this.board[x][y], x, y);
            }
        }
    }

}

bool _isPiece(int squareContent) => -7 < squareContent < 0 || 0 < squareContent < 7;

bool _isPawn(int squareContent) => squareContent == 1 || squareContent == -1;

bool _isKnight(int squareContent) => squareContent == 2 || squareContent == -2;

bool _isRook(int squareContent) => squareContent == 4 || squareContent == -4;

bool _isKing(int squareContent) => squareContent == 6 || squareContent == -6;

namespace Program
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
