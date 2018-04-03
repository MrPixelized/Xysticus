using System;
using System.Collections.Generic;

namespace Chess
{

    public class Position
    {
        public int toMove;
        public int fiftyMoveProximity;
        public int[,] board;
        public bool[] castlingRights;

        public Position(int[,] board = null, int toMove = 1, int fiftyMoveProximity = 0, bool[] castlingRights = null)
        {
            this.board = board ?? Constants.STANDARD_POSITION;
            this.toMove = toMove;
            this.fiftyMoveProximity = fiftyMoveProximity;
            this.castlingRights = castlingRights ?? Constants.STANDARD_CASTLING_RIGHTS;
        }

        public float FindBestMove(int depth, float alpha, float beta)
        {
            float bestEvaluation = 2 * toMove;
            float evaluation;

            if (toMove == -1)
            {
                foreach ((Position position, Move move) in GeneratePositions())
                {
                    if (!position._kingsInPosition) continue;

                    if (depth > 1) evaluation = position.FindBestMove(depth - 1, alpha, beta);
                    // implement evaluation network here, for now simply random value
                    else evaluation = (float)new Random().NextDouble();

                    bestEvaluation = Math.Max(bestEvaluation, evaluation);
                    alpha = Math.Max(bestEvaluation, beta);
                    if (beta <= alpha) break;
                }
            }

            else
            {
                foreach ((Position position, Move move) in GeneratePositions())
                {
                    if (!position._kingsInPosition) continue;

                    if (depth > 1) evaluation = position.FindBestMove(depth - 1, alpha, beta);
                    // implement eveluation network here, for now simply random value
                    else evaluation = (float)new Random().NextDouble();

                    bestEvaluation = Math.Min(bestEvaluation, evaluation);
                    beta = Math.Min(bestEvaluation, beta);
                    if (beta <= alpha) break;
                }
            }

            return bestEvaluation;
        }

        public IEnumerable<(Position, Move)> GeneratePositions()
            {
                foreach ((int squareContent, int x, int y) in _iteratePosition())
                {
                    if (_isPiece(squareContent) && squareContent * toMove > 0)
                    {
                        foreach ((int moveX, int moveY) in Constants.MOVE_DIRECTIONS(squareContent))
                        {
                            for (int m = 1; m < 9; m++)
                            {
                                int toX = x + moveX * m;
                                int toY = y + moveY * m;
                                Move currentMove = new Move(x, y, toX, toY);

                                if (!_legalMove(currentMove)) break;

                                yield return (MakeMove(currentMove), currentMove);

                                if (_isKing(squareContent) || _isKnight(squareContent) || _isPawn(squareContent)) break;
                                if (_isPiece(board[toX, toY])) break;
                            }
                        }
                    }
                }
            }

        public Position MakeMove(Move move)
            {
                int toX = move.toX;
                int toY = move.toY;
                int fromX = move.fromX;
                int fromY = move.fromY;

                int pieceToMove = board[fromX, fromY];
                int squareToMoveTo = board[toX, toY];

                int moveDifX = fromX - toX;
                int moveDifY = fromY - toY;

                int[,] newBoard = (int[,]) board.Clone();
                bool[] newCastlingRights = (bool[]) castlingRights.Clone();
                int newToMove = -1 * toMove;
                int newFiftyMoveProximity = fiftyMoveProximity;

                if (_isPiece(squareToMoveTo)) newFiftyMoveProximity = 0;

                if (_isKing(pieceToMove))
                {
                    if (pieceToMove < 0)
                    {
                        (newCastlingRights[1], newCastlingRights[3]) = (false, false);
                        if (moveDifX == -2)
                        {
                            newBoard[fromX + 1, 0] = -4;
                            newBoard[0, 7] = 0;
                        }
                        else if (moveDifX == 2)
                        {
                            newBoard[fromX - 1, 0] = -4;
                            newBoard[0, 0] = 0;
                        }
                    }
                    else
                    {
                        (newCastlingRights[0], newCastlingRights[2]) = (false, false);
                        if (moveDifX == -2)
                        {
                            newBoard[fromX + 1, 7] = 4;
                            newBoard[7, 7] = 0;
                        }
                        else if (moveDifX == 2)
                        {
                            newBoard[fromX - 1, 7] = 4;
                            newBoard[0, 7] = 4;
                        }
                    }
                }

                if (_isRook(pieceToMove))
                {
                    if (fromX == 0 || fromY == 0) newCastlingRights[3] = false;
                    else if (fromX == 0 || fromY == 7) newCastlingRights[2] = false;
                    else if (fromX == 7 || fromY == 0) newCastlingRights[1] = false;
                    else if (fromX == 7 || fromY == 7) newCastlingRights[0] = false;
                }

                if (_isEnPassant(squareToMoveTo) || _isPawn(pieceToMove))
                {
                    if (pieceToMove < 0) newBoard[toX, toY - 1] = 0;
                    else newBoard[toX, toY + 1] = 0;
                }

                _removeEnPassant(ref newBoard);

                if (_isPawn(pieceToMove))
                {
                    newFiftyMoveProximity = 0;
                    if (moveDifY == -2) newBoard[fromX, fromY + 1] = -7;
                    else if (moveDifY == 2) newBoard[fromX, fromY - 1] = 7;
                }

                (newBoard[fromX, fromY], newBoard[toX, toY]) = (0, pieceToMove);

                return new Position(newBoard, newToMove, newFiftyMoveProximity, newCastlingRights);
            }

        private bool _legalMove(Move move)
            {
                int toX = move.toX;
                int toY = move.toY;

                if (toX > 7 || toX < 0 || toY > 7 || toY < 0)
                {
                    return false;
                }

                int fromY = move.fromY;
                int fromX = move.fromX;

                int pieceToMove = board[fromX, fromY];
                int squareToMoveTo = board[toX, toY];
                int colorIndication = pieceToMove * squareToMoveTo;

                int moveDifX = fromX - toX;
                int moveDifY = fromY - toY;

                if (_isPawn(pieceToMove))
                {
                    if (moveDifY == 2 && !(fromY == 6))
                    {
                        return false;
                    }
                    else if (moveDifY == -2 && !(fromY == 1))
                    {
                        return false;
                    }
                    else if ((moveDifX == 1 || moveDifX == -1) && colorIndication >= 0)
                    {
                        return false;
                    }
                }

                else if (_isKing(pieceToMove))
                {
                    if (pieceToMove < 0)
                    {
                        if (moveDifX == 2)
                        {
                            if (!(board[5, 0] == 0 || board[6, 0] == 0 || castlingRights[1]))
                            {
                                return false;
                            }
                        }
                        else if (moveDifX == -2)
                        {
                            if (!(board[1, 0] == 0 || board[2, 0] == 0 || board[3, 0] == 0 || castlingRights[3]))
                            {
                                return false;
                            }
                        }
                    }
                    else
                    {
                        if (moveDifX == 2)
                        {
                            if (!(board[5, 7] == 0 || board[6, 7] == 0 || castlingRights[0]))
                            {
                                return false;
                            }
                        }
                        else if (moveDifX == -2)
                        {
                            if (!(board[1, 7] == 0 || board[2, 7] == 0 || board[3, 7] == 0 || castlingRights[2]))
                            {
                                return false;
                            }
                        }
                    }
                }

                return colorIndication <= 0;
            }

        private bool _kingsInPosition
            {
                get
                {
                    int kingCount = 0;
                    for (int x = 0; x < 8; x++)
                    {
                        for (int y = 0; y < 8; y++)
                        {
                            switch (board[x, y])
                            {
                                case -6: kingCount++; break;
                                case 6: kingCount++; break;
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

        #region Short aliases

        static bool _isPiece(int squareContent) => (-7 < squareContent && squareContent < 0) || (0 < squareContent && squareContent < 7);

        static bool _isPawn(int squareContent) => squareContent == 1 || squareContent == -1;

        static bool _isKnight(int squareContent) => squareContent == 2 || squareContent == -2;

        static bool _isRook(int squareContent) => squareContent == 4 || squareContent == -4;

        static bool _isKing(int squareContent) => squareContent == 6 || squareContent == -6;

        static bool _isEnPassant(int squareContent) => squareContent == 7 || squareContent == -7;

        static void _removeEnPassant(ref int[,] board)
        {
            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    if (_isEnPassant(board[x, y]))
                    {
                        board[x, y] = 0;
                        return;
                    }
                }
            }
        }

        #endregion

    }

}
