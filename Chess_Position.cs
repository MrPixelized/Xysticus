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
        private Position[] reachablePositions;

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

            // Maximizing player
            if (toMove == -1)
            {
                foreach ((Position position, Move move) in GeneratePositions())
                {
                    // If a king can be captured in this position, make sure that the engine never chooses this position
                    if (!position._kingsInPosition) evaluation = -3;

                    // If the requested depth has not yet been reached, generate another layer of positions
                    // Otherwise, evaluate this current position (which is a leaf node)
                    if (depth > 1) evaluation = position.FindBestMove(depth - 1, alpha, beta);
                    // FUTURE: Implement evaluation network here, for now simply random value
                    else evaluation = (float) new Random().NextDouble();

                    // MINIMAX
                    bestEvaluation = Math.Max(bestEvaluation, evaluation);
                    alpha = Math.Max(bestEvaluation, beta);
                    if (beta <= alpha) break;
                }
            }

            // Minimizing player
            else
            {
                foreach ((Position position, Move move) in GeneratePositions())
                {
                    // If a king can be captured in this position, make sure that the engine never chooses this position
                    if (!position._kingsInPosition) evaluation = -3;

                    // If the requested depth has not yet been reached, generate another layer of positions
                    // Otherwise, evaluate this current position (which is a leaf node)
                    if (depth > 1) evaluation = position.FindBestMove(depth - 1, alpha, beta);
                    // FUTURE: Implement eveluation network here, for now simply random value
                    else evaluation = (float)new Random().NextDouble();

                    // MINIMAX
                    bestEvaluation = Math.Min(bestEvaluation, evaluation);
                    beta = Math.Min(bestEvaluation, beta);
                    if (beta <= alpha) break;
                }
            }

            return bestEvaluation;
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
                            if (!_legalMove(currentMove)) break;

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

            int moveDifX = fromX - toX;
            int moveDifY = fromY - toY;

            int[,] newBoard = (int[,]) board.Clone();
            bool[] newCastlingRights = (bool[]) castlingRights.Clone();
            int newToMove = -1 * toMove;
            int newFiftyMoveProximity = fiftyMoveProximity;
            
            // If this move is a capturing move, reset the fifty move proximity.
            if (_isPiece(squareToMoveTo)) newFiftyMoveProximity = 0;

            // If a king is to castle, the rook is moved in advance and castling rights are altered accordingly
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
                if (_isEnPassant(squareToMoveTo))
                {
                    if (pieceToMove < 0) newBoard[toX, toY - 1] = 0;
                    else newBoard[toX, toY + 1] = 0;
                }
            }

            // Removing the en passant square
            _removeEnPassant(ref newBoard);

            // Exceptions for double pawn moves are made
            if (_isPawn(pieceToMove))
            { 
                if (moveDifY == -2) newBoard[fromX, fromY + 1] = -7;
                else if (moveDifY == 2) newBoard[fromX, fromY - 1] = 7;

                newFiftyMoveProximity = 0;
            }
            
            // Finally, the piece is moved to the desired spot
            (newBoard[fromX, fromY], newBoard[toX, toY]) = (0, pieceToMove);

            return new Position(newBoard, newToMove, newFiftyMoveProximity, newCastlingRights);
        }

        private bool _legalMove(Move move)
        {
            int toX = move.toX;
            int toY = move.toY;

            // Testing to see if the move puts a piece out of bounds
            if (_outOfBounds(toX, toY)) return false;

            int fromY = move.fromY;
            int fromX = move.fromX;

            int pieceToMove = board[fromX, fromY];
            int squareToMoveTo = board[toX, toY];
            int colorIndication = pieceToMove * squareToMoveTo;

            int moveDifX = fromX - toX;
            int moveDifY = fromY - toY;

            // Make sure the piece moved doesn't land on a friendly piece
            if (colorIndication > 0) return false;

            // Test to see if any double pawn moves or pawn pushes are legal
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

            // If a king might want to castle, test if this is in accordance with the current castling rights
            else if (_isKing(pieceToMove))
            {
                if (_castlingLegal(moveDifX, pieceToMove)) return true;
                return false;
            }

            return true;
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

        static bool _outOfBounds(int x, int y)
        {
            return (x > 7 || x < 0 || y > 7 || y < 0);
        }

        bool _castlingLegal(int moveDifX, int king)
        {
            // Black to castle
            if (king < 0)
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
