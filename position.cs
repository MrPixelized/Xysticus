using System;
using System.Collections.Generic;

class Program {

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

public static (int, int)[] MOVE_DIRECTIONS(int index) {
  switch (index) {
    case -6: return new (int, int)[] { ( 1, 1 ), ( -1,1 ), (-1,-1 ), ( 1,-1 ), ( 0, 1 ), ( 0,-1 ), ( 1, 0 ), ( -1,0 ), ( 2, 0 ), ( -2,0 ) };
    case -5: return new (int, int)[] { ( 1, 1 ), ( -1,1 ), (-1,-1 ), ( 1,-1 ), ( 0, 1 ), ( 0,-1 ), ( 1, 0 ), ( -1,0 ) };
    case -4: return new (int, int)[] { ( 0, 1 ), ( 0,-1 ), ( 1, 0 ), ( -1,0 ) };
    case -3: return new (int, int)[] { ( 1, 1 ), ( -1,1 ), (-1,-1 ), ( 1,-1 ) };
    case -2: return new (int, int)[] { ( 1, 2 ), ( -1,2 ), ( 1,-2 ), (-1,-2 ) };
    case -1: return new (int, int)[] { ( 0, 1 ), ( 1, 1 ), ( -1,1 ), ( 0, 2 ) };
    case  1: return new (int, int)[] { ( 0,-1 ), ( 1,-1 ), (-1,-1 ), ( 0,-2 ) };
    case  2: return new (int, int)[] { ( 1, 2 ), ( -1,2 ), ( 1,-2 ), (-1,-2 ) };
    case  3: return new (int, int)[] { ( 1, 1 ), ( -1,1 ), (-1,-1 ), ( 1,-1 ) };
    case  4: return new (int, int)[] { ( 0, 1 ), ( 0,-1 ), ( 1, 0 ), ( -1,0 ) };
    case  5: return new (int, int)[] { ( 1, 1 ), ( -1,1 ), (-1,-1 ), ( 1,-1 ), ( 0, 1 ), ( 0,-1 ), ( 1, 0 ), ( -1,0 ) };
    case  6: return new (int, int)[] { ( 1, 1 ), ( -1,1 ), (-1,-1 ), ( 1,-1 ), ( 0, 1 ), ( 0,-1 ), ( 1, 0 ), ( -1,0 ), ( 2, 0 ), ( -2,0 ) };
  }
  return null;
}

public static readonly bool[] STANDARD_CASTLING_RIGHTS = new bool[4] { true, true, true, true };

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
	
	public Position(int[,] board = null, int toMove = 1, int fiftyMoveProximity = 0, bool[] castlingRights = null) {
		this.board = board ?? STANDARD_POSITION;
		this.toMove = toMove;
		this.fiftyMoveProximity = fiftyMoveProximity;
		this.castlingRights = castlingRights ?? STANDARD_CASTLING_RIGHTS;
	}

    public float FindBestMove(int depth, float alpha, float beta) {
        float bestEvaluation = 2 * toMove;
        float evaluation;

        if (toMove == -1)
        {
            foreach ((Position position, Move move) in GeneratePositions()) {
                if (!position._kingsInPosition)
                {
                    continue;
                }
                if (depth > 1)
                {
                    evaluation = position.FindBestMove(depth - 1, alpha, beta);
                }
                else
                {
                    evaluation = (float)new Random().NextDouble(); /* implement evaluation network here, for now simply random value */ 
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
            foreach ((Position position, Move move) in GeneratePositions()) {
                if (!position._kingsInPosition)
                {
                    continue;
                }
                if (depth > 1)
                {
                    evaluation = position.FindBestMove(depth - 1, alpha, beta);
                }
                else
                {
                    evaluation = (float) new Random().NextDouble(); /* implement eveluation network here, for now simply random value */
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
    
    public string toString()
    {
        return "DORK";
    }

    private IEnumerable<(Position, Move)> GeneratePositions()
    {
        foreach ((int squareContent, int x, int y) in _iteratePosition())
        {
            if (_isPiece(squareContent) && squareContent * toMove > 0)
            {
                foreach ((int moveX, int moveY) in MOVE_DIRECTIONS(squareContent))
                {
                    for (int m = 1; m < 9; m++)
                    {
                        int toX = x + moveX * m;
                        int toY = y + moveY * m;
                        if (!_legalMove(x, y, toX, toY))
                        {
                            break;
                        }
                        else
                        {
                            yield return (MakeMove(x, y, toX, toY), new Move (x, y, toX, toY));
                        }
                        if (_isKing(squareContent) || _isKnight(squareContent) || _isPawn(squareContent))
                        {
                            break;
                        }
                        if (_isPiece(board[toX, toY]))
                        {
                            break;
                        }
                    }
                }
            }
        }
    }

    public Position MakeMove(int x, int y, int toX, int toY)
    {
        return new Position();
    }

    private bool _legalMove(int x, int y, int toX, int toY) => true;

    private bool _kingsInPosition
    {
        get
        {
            int kingCount = 0;
            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    switch (board[x,y])
                    {
                        case -6: kingCount++;break;
                        case  6: kingCount++;break;
                    }
                }
            }

            return (kingCount == 2);
        }
    }

    private IEnumerable<(int, int, int)> _iteratePosition()
    {
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                yield return (board[x, y], x, y);
            }
        }
    }

}

static bool _isPiece(int squareContent) => (-7 < squareContent && squareContent < 0) || (0 < squareContent && squareContent < 7);

static bool _isPawn(int squareContent) => squareContent == 1 || squareContent == -1;

static bool _isKnight(int squareContent) => squareContent == 2 || squareContent == -2;

static bool _isRook(int squareContent) => squareContent == 4 || squareContent == -4;

static bool _isKing(int squareContent) => squareContent == 6 || squareContent == -6;

    static void Main()
    {
        Position a = new Position();
        foreach ((Position position, Move move) in a.GeneratePositions())
        {
            Console.WriteLine(position);
        }
        Console.ReadKey();
    }
}
