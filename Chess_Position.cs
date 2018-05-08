using System;
using System.Collections.Generic;
using Interface;

namespace Chess
{
    public class Position
    {
        public int toMove;
        public int fiftyMoveProximity;
        public int[,] board;
        public bool[] castlingRights;
        public Tuple<int, int> enPassantSquare;

        public Position(int[,] board = null, int toMove = 1, int fiftyMoveProximity = 0, bool[] castlingRights = null, Tuple<int, int> enPassantSquare = null)
        {
            this.board = board ?? Constants.STANDARD_POSITION;
            this.toMove = toMove;
            this.fiftyMoveProximity = fiftyMoveProximity;
            this.castlingRights = castlingRights ?? Constants.STANDARD_CASTLING_RIGHTS;
            this.enPassantSquare = enPassantSquare ?? new Tuple<int, int>(-1, -1);
        }
        public IEnumerable<(Position, Move)> GeneratePositions()
        {
            // Iterate through each square on the board
            foreach ((int squareContent, int x, int y) in _iteratePosition())
            {
                // Check if the resulting square contains a piece that can be moved
                if (_isPiece(squareContent) && squareContent * toMove > 0)
                {
                    // Request the possible moves for this piece and loop through them
                    foreach ((int moveX, int moveY) in Constants.MOVE_DIRECTIONS(squareContent))
                    {
                        for (int m = 1; m < 9; m++)
                        {
                            int toX = x + moveX * m;
                            int toY = y + moveY * m;
                            Move currentMove = new Move(x, y, toX, toY);

                            // Test if the current move being computed is possible, disregarding checks.
                            if (!IsLegalMove(currentMove)) break;

                            // If the square this piece is about to land on is a king, recognise that this move is illegal by returning null
                            if (_isKing(board[toX, toY])) yield return (null, currentMove);

                            yield return (MakeMove(currentMove), currentMove);

                            // Quit yielding new positions if the piece is a pawn, king, or knight, or if a capture has occured
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

            int moveDifX = toX - fromX;
            int moveDifY = toY - fromY;

            int[,] newBoard = (int[,])board.Clone();
            bool[] newCastlingRights = (bool[])castlingRights.Clone();
            int newToMove = -1 * toMove;
            int newFiftyMoveProximity = fiftyMoveProximity + 1;
            Tuple<int, int> newEnPassantSquare = new Tuple<int, int>(-1, -1);

            // If this move is a capturing move, reset the fifty move proximity.
            if (_isPiece(squareToMoveTo)) newFiftyMoveProximity = 0;

            // If a king is to castle, the rook is moved in advance and castling rights are altered accordingly
            if (_isKing(pieceToMove))
            {
                if (pieceToMove < 0)
                {
                    (newCastlingRights[1], newCastlingRights[3]) = (false, false);
                    if (moveDifX == 2)
                    {
                        newBoard[fromX + 1, 0] = -4;
                        newBoard[7, 0] = 0;
                    }
                    else if (moveDifX == -2)
                    {
                        newBoard[fromX - 1, 0] = -4;
                        newBoard[0, 0] = 0;
                    }
                }
                else
                {
                    (newCastlingRights[0], newCastlingRights[2]) = (false, false);
                    if (moveDifX == 2)
                    {
                        newBoard[fromX + 1, 7] = 4;
                        newBoard[7, 7] = 0;
                    }
                    else if (moveDifX == -2)
                    {
                        newBoard[fromX - 1, 7] = 4;
                        newBoard[0, 7] = 4;
                    }
                }
            }

            // Altering the castling rights upon a rook move
            if (_isRook(pieceToMove))
            {
                if (fromX == 0 || fromY == 0) newCastlingRights[3] = false;
                else if (fromX == 0 || fromY == 7) newCastlingRights[2] = false;
                else if (fromX == 7 || fromY == 0) newCastlingRights[1] = false;
                else if (fromX == 7 || fromY == 7) newCastlingRights[0] = false;
            }

            // If the piece to move is a pawn, fifty move proximity is reset
            // Also, en passant capturing is handled
            if (_isPawn(pieceToMove))
            {
                if (enPassantSquare.Item1 == toX && enPassantSquare.Item2 == toY)
                {
                    if (pieceToMove < 0) newBoard[toX, toY - 1] = 0;
                    else newBoard[toX, toY + 1] = 0;
                    newBoard[fromX, fromY] = 0;
                    newBoard[toX, toY] = pieceToMove;
                }

                if (moveDifY == 2)
                {
                    newEnPassantSquare = new Tuple<int, int>(fromX, 2);
                }
                else if (moveDifY == -2)
                {
                    newEnPassantSquare = new Tuple<int, int>(fromX, 5);
                }

                newFiftyMoveProximity = 0;
            }

            // Finally, the piece is moved to the desired spot
            newBoard[fromX, fromY] = 0;
            newBoard[toX, toY] = pieceToMove;

            return new Position(newBoard, newToMove, newFiftyMoveProximity, newCastlingRights, newEnPassantSquare);
        }

        private bool IsLegalMove(Move move)
        {
            int toX = move.toX;
            int toY = move.toY;

            // Testing to see if the move puts a piece out of bounds
            if (_isOutOfBounds(toX, toY)) return false;

            int fromY = move.fromY;
            int fromX = move.fromX;

            int pieceToMove = board[fromX, fromY];
            int squareToMoveTo = board[toX, toY];

            int moveDifX = toX - fromX;
            int moveDifY = toY - fromY;


            // Test to see if any double pawn moves or pawn pushes are legal
            if (_isPawn(pieceToMove))
            {
                if (moveDifX != 0)
                {
                    if (squareToMoveTo * pieceToMove < 0)
                    {
                        return true;
                    }

                    if (enPassantSquare.Item1 == toX && enPassantSquare.Item2 == toY)
                    {
                        return true;
                    }
                }
                else
                {
                    if (Math.Abs(moveDifY) == 2 && !(fromY == 1 || fromY == 6)) return false;
                    if (board[fromX, fromY + Math.Sign(moveDifY)] != 0)
                    {
                        return false;
                    }
                    if (moveDifY == 1 || moveDifY == -1) return true;
                    if (moveDifY == 2 || moveDifY == -2) return squareToMoveTo == 0;
                }

            }

            // If a king might want to castle, test if this is in accordance with the current castling rights
            else if (_isKing(pieceToMove) && (moveDifX == 2 || moveDifX == -2))
            {
                return _isLegalCastling(moveDifX, pieceToMove);
            }
            else
            {
                return squareToMoveTo * pieceToMove <= 0;
            }

            return false;
        }

        #region Short aliases

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

        private static bool _isPiece(int squareContent) => (-7 < squareContent && squareContent < 0) || (0 < squareContent && squareContent < 7);

        private static bool _isPawn(int squareContent) => squareContent == 1 || squareContent == -1;

        private static bool _isKnight(int squareContent) => squareContent == 2 || squareContent == -2;

        private static bool _isRook(int squareContent) => squareContent == 4 || squareContent == -4;

        private static bool _isKing(int squareContent) => squareContent == 6 || squareContent == -6;

        private static bool _isOutOfBounds(int x, int y) => (x > 7 || x < 0 || y > 7 || y < 0);

        private bool _isLegalCastling(int moveDifX, int king)
        {
            // Black to castle
            if (king < 0)
            {
                // Short castling
                if (moveDifX == -2)
                {
                    if (board[5, 0] == 0 && board[6, 0] == 0 && castlingRights[1])
                    {
                        return true;
                    }
                }
                // Long castling
                else if (moveDifX == 2)
                {
                    if (board[1, 0] == 0 && board[2, 0] == 0 && board[3, 0] == 0 && castlingRights[3])
                    {
                        return true;
                    }
                }
            }

            // White to castle
            else
            {
                // Short castling
                if (moveDifX == -2)
                {
                    if (board[5, 7] == 0 && board[6, 7] == 0 && castlingRights[0])
                    {
                        return true;
                    }
                }
                // Long castling
                else if (moveDifX == 2)
                {
                    if (board[1, 7] == 0 && board[2, 7] == 0 && board[3, 7] == 0 && castlingRights[2])
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        #endregion

    }

}
