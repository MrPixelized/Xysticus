using System;
using System.Collections.Generic;
using System.Linq;
using static Chess.Constants;

namespace Chess
{
    public class Position
    {
        public int toMove;
        public int fiftyMoveProximity;
        public int[,] board;
        public bool[] castlingRights;
        public Tuple<int, int> enPassantSquare;
        public bool? inCheck;

        public Position(int[,] board = null, int toMove = 1, int fiftyMoveProximity = 0, bool[] castlingRights = null, Tuple<int, int> enPassantSquare = null)
        {
            this.board = board ?? STANDARD_POSITION;
            this.toMove = toMove;
            this.fiftyMoveProximity = fiftyMoveProximity;
            this.castlingRights = castlingRights ?? STANDARD_CASTLING_RIGHTS;
            this.enPassantSquare = enPassantSquare ?? new Tuple<int, int>(-1, -1);
        }
        public List<(Position, Move)> GeneratePositions()
        {
            List<(Position, Move)> positionMoveList = new List<(Position, Move)>();
            if (toMove == WHITE)
            {
                // Iterate through each square on the board
                foreach ((int squareContent, int x, int y) in _iteratePosition())
                {
                    // Check if the resulting square contains a piece that can be moved
                    if (squareContent > 0)
                    {
                        // Request the possible moves for this piece and loop through them
                        foreach ((int moveX, int moveY) in MOVE_DIRECTIONS(squareContent))
                        {
                            for (int m = 1; m < 9; m++)
                            {
                                Move currentMove;
                                int toX = x + moveX * m;
                                int toY = y + moveY * m;
                                if (_isPawn(squareContent))
                                {
                                    if (toX == enPassantSquare.Item1 && toY == enPassantSquare.Item2)
                                    {
                                        currentMove = new Move(x, y, toX, toY, squareContent, 0, true);
                                    }
                                    else if (toY == 0)
                                    {
                                        for (int piece = 2; piece <= 5; piece++)
                                        {
                                            currentMove = new Move(x, y, toX, toY, squareContent, piece, false);
                                            if (!IsLegalMove(currentMove)) break;
                                            if (_isKing(board[toX, toY]))
                                            {
                                                positionMoveList.Add((null, currentMove));
                                                return positionMoveList;
                                            }
                                            positionMoveList.Add((MakeMove(currentMove), currentMove));
                                        }
                                        break;
                                    }
                                    else
                                    {
                                        currentMove = new Move(x, y, toX, toY, squareContent);
                                    }
                                }
                                else
                                {
                                    currentMove = new Move(x, y, toX, toY, squareContent);
                                }

                                // Test if the current move being computed is possible, disregarding checks
                                if (!IsLegalMove(currentMove)) break;

                                // If the square this piece is about to land on is a king, recognise that this position is illegal by returning null
                                if (_isKing(board[toX, toY]))
                                {
                                    positionMoveList.Add((null, currentMove));
                                    return positionMoveList;
                                }

                                positionMoveList.Add((MakeMove(currentMove), currentMove));

                                // Quit yielding new positions if the piece is a pawn, king, or knight, or if a capture has occured
                                if (_isKing(squareContent) || _isKnight(squareContent) || _isPawn(squareContent)) break;
                                if (_isPiece(board[toX, toY])) break;
                            }
                        }
                    }
                }
            }
            else
            {
                // Iterate through each square on the board
                foreach ((int squareContent, int x, int y) in _iteratePosition())
                {
                    // Check if the resulting square contains a piece that can be moved
                    if (squareContent < 0)
                    {
                        // Request the possible moves for this piece and loop through them
                        foreach ((int moveX, int moveY) in MOVE_DIRECTIONS(squareContent))
                        {
                            for (int m = 1; m < 9; m++)
                            {
                                Move currentMove;
                                int toX = x + moveX * m;
                                int toY = y + moveY * m;
                                if (_isPawn(squareContent))
                                {
                                    if (toX == enPassantSquare.Item1 && toY == enPassantSquare.Item2)
                                    {
                                        currentMove = new Move(x, y, toX, toY, squareContent, 0, true);
                                    }
                                    else if (toY == 7)
                                    {
                                        for (int piece = -2; piece >= -5; piece--)
                                        {
                                            currentMove = new Move(x, y, toX, toY, squareContent, piece, false);
                                            if (!IsLegalMove(currentMove)) break;
                                            if (_isKing(board[toX, toY]))
                                            {
                                                positionMoveList.Add((null, currentMove));
                                                return positionMoveList;
                                            }
                                            positionMoveList.Add((MakeMove(currentMove), currentMove));
                                        }
                                        break;
                                    }
                                    else
                                    {
                                        currentMove = new Move(x, y, toX, toY, squareContent);
                                    }
                                }
                                else
                                {
                                    currentMove = new Move(x, y, toX, toY, squareContent);
                                }

                                // Test if the current move being computed is possible, disregarding checks
                                if (!IsLegalMove(currentMove)) break;

                                // If the square this piece is about to land on is a king, recognise that this position is illegal by returning null
                                if (_isKing(board[toX, toY]))
                                {
                                    positionMoveList.Add((null, currentMove));
                                    return positionMoveList;
                                }

                                positionMoveList.Add((MakeMove(currentMove), currentMove));

                                // Quit yielding new positions if the piece is a pawn, king, or knight, or if a capture has occured
                                if (_isKing(squareContent) || _isKnight(squareContent) || _isPawn(squareContent)) break;
                                if (_isPiece(board[toX, toY])) break;
                            }
                        }
                    }
                }
            }
            return positionMoveList;
        }

        public Position MakeMove(Move move)
        {
            int toX = move.toX;
            int toY = move.toY;
            int fromX = move.fromX;
            int fromY = move.fromY;

            int pieceToMove = move.pieceToMove;
            int squareToMoveTo = board[toX, toY];
            int newPiece = move.newPiece;

            int moveDifX = toX - fromX;
            int moveDifY = toY - fromY;

            int[,] newBoard = (int[,])board.Clone();
            bool[] newCastlingRights = (bool[])castlingRights.Clone();
            int newToMove = -1 * toMove;
            int newFiftyMoveProximity = fiftyMoveProximity + 1;
            Tuple<int, int> newEnPassantSquare = new Tuple<int, int>(-1, -1);

            // If this move is a capturing move, reset the fifty move proximity
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
            else if (move.enPassantCapture)
            {
                newBoard[toX, toY + pieceToMove] = 0;
                newFiftyMoveProximity = 0;
            }

            else if (_isPawn(pieceToMove))
            {
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
            newBoard[toX, toY] = newPiece;

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

            int pieceToMove = move.pieceToMove;
            int squareToMoveTo = board[toX, toY];

            int moveDifX = toX - fromX;
            int moveDifY = toY - fromY;

            if (toMove == BLACK)
            {

                // Test to see if any double pawn moves or pawn pushes are legal
                if (_isPawn(pieceToMove))
                {
                    if (moveDifY == 2)
                    {
                        return board[fromX, fromY + 1] == 0 && squareToMoveTo == 0 && fromY == 1;
                    }
                    else if (moveDifX == 0)
                    {
                        return squareToMoveTo == 0;
                    }
                    else
                    {
                        if (squareToMoveTo > 0)
                        {
                            return true;
                        }

                        if (move.enPassantCapture)
                        {
                            return true;
                        }
                    }
                }

                // If a king might want to castle, test if this is in accordance with the current castling rights
                else if (pieceToMove == BLACK_KING && (moveDifX == 2 || moveDifX == -2))
                {
                    return _isLegalCastling(moveDifX, BLACK);
                }
                else
                {
                    return squareToMoveTo >= 0;
                }
            }
            else
            {
                // Test to see if any double pawn moves or pawn pushes are legal
                if (_isPawn(pieceToMove))
                {
                    if (moveDifY == -2)
                    {
                        return board[fromX, fromY - 1] == 0 && squareToMoveTo == 0 && fromY == 6;
                    }
                    else if (moveDifX == 0)
                    {
                        return squareToMoveTo == 0;
                    }
                    else
                    {
                        if (squareToMoveTo < 0)
                        {
                            return true;
                        }

                        if (move.enPassantCapture)
                        {
                            return true;
                        }
                    }
                }

                // If a king might want to castle, test if this is in accordance with the current castling rights
                else if (pieceToMove == WHITE_KING && (moveDifX == 2 || moveDifX == -2))
                {
                    return _isLegalCastling(moveDifX, WHITE);
                }
                else
                {
                    return squareToMoveTo <= 0;
                }
            }
            return false;
        }

        public bool IsInCheck()
        {
            if (inCheck != null) return (bool)inCheck;
            Position positionWithOppositeSideToMove = new Position()
            {
                toMove = -1 * this.toMove,
                board = this.board
            };
            positionWithOppositeSideToMove.toMove *= -1;
            if (positionWithOppositeSideToMove.GeneratePositions().Last().Item1 == null) return true;
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

        private static bool _isPiece(int squareContent) => (squareContent != EMPTY_SQUARE);

        private static bool _isPawn(int squareContent) => squareContent == WHITE_PAWN || squareContent == BLACK_PAWN;

        private static bool _isKnight(int squareContent) => squareContent == WHITE_KNIGHT || squareContent == BLACK_KNIGHT;

        private static bool _isRook(int squareContent) => squareContent == WHITE_ROOK || squareContent == BLACK_ROOK;

        private static bool _isKing(int squareContent) => squareContent == WHITE_KING || squareContent == BLACK_KING;

        private static bool _isOutOfBounds(int x, int y) => (x > 7 || x < 0 || y > 7 || y < 0);

        private bool _isLegalCastling(int moveDifX, int toMove)
        {
            // Black to castle
            if (toMove == BLACK)
            {
                // Short castling
                if (moveDifX == 2)
                {
                    if (board[5, 0] == 0 && board[6, 0] == 0 && castlingRights[1])
                    {
                        return true;
                    }
                }
                // Long castling
                else if (moveDifX == -2)
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
                if (moveDifX == 2)
                {
                    if (board[5, 7] == 0 && board[6, 7] == 0 && castlingRights[0])
                    {
                        return true;
                    }
                }
                // Long castling
                else if (moveDifX == -2)
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
