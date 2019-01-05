using System;
using System.Collections.Generic;
using Chess;
using static Interface.Constants;

namespace Interface
{
    public class FENParser
    {
        #region FEN string parts
        private static int[,] FlipBoardSideways(int[,] board)        
        {
            int[,] newBoard = new int[8, 8];
            for(int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    newBoard[i, j] = board[j, i];
                }
            }
            return newBoard;
        }

        private static int[,] ParseBoard(string FENBoard)
        {
            int[,] board = new int[8,8];
            string[] FENBoardRanks = FENBoard.Split('/');
            int fileNumber;

            for (int rankNumber = 0; rankNumber < 8; rankNumber++)
            {
                fileNumber = 0;
                foreach (char character in FENBoardRanks[rankNumber])
                {
                    if (Char.IsNumber(character))
                    {
                        for (int k = 0; k < Char.GetNumericValue(character); k++)
                        {
                            board[rankNumber, fileNumber] = 0;
                            fileNumber++;
                        }
                    }
                    else
                    {
                        board[rankNumber, fileNumber] = INVERSED_PIECE_REPRESENTATIONS(character);
                        fileNumber++;
                    }
                }
            }
            board = FlipBoardSideways(board);
            return board;
        }
        private static int ParseToMove(string toMoveString)
        {
            int toMove;

            if (toMoveString == "w")
            {
                toMove = 1;
            }
            else
            {
                toMove = -1;
            }
            return toMove;
        }
        private static bool[] ParseCastlingRights(string CastlingRightsString)
        {
            bool[] castlingRights = new bool[4] {false, false, false, false};
            if (CastlingRightsString.Contains("K")) castlingRights[0] = true;
            if (CastlingRightsString.Contains("k")) castlingRights[1] = true;
            if (CastlingRightsString.Contains("Q")) castlingRights[2] = true;
            if (CastlingRightsString.Contains("q")) castlingRights[3] = true;
            return castlingRights;
        }
        private static Tuple<int, int> ParseEnPassantSquare(string enPassantSquareString)
        {
            Tuple<int, int> enPassantSquare;

            if (!(enPassantSquareString == "-")) {
                enPassantSquare = new Tuple<int, int>(COORDINATE_TRANSFORMATION(enPassantSquareString[0]), 8 - (int)Char.GetNumericValue(enPassantSquareString[1]));
            }
            else
            {
                enPassantSquare = new Tuple<int, int>(-1, -1);
            }
            return enPassantSquare;
        }
        private static int ParseFiftyMoveProximity(string fiftyMoveProximityString)
        {
            int fiftyMoveProximity = Convert.ToInt32(fiftyMoveProximityString);
            return fiftyMoveProximity;
        }
        #endregion

        #region Main function
        public static Position ParseFEN(string[] FENStringArray)
        {
            int[,] board = ParseBoard(FENStringArray[0]);
            int toMove = ParseToMove(FENStringArray[1]);
            bool[] castlingRights = ParseCastlingRights(FENStringArray[2]);
            Tuple<int, int> enPassantSquare = ParseEnPassantSquare(FENStringArray[3]);
            int fiftyMoveProximity = ParseFiftyMoveProximity(FENStringArray[4]);
            return new Position(board, toMove, fiftyMoveProximity, castlingRights, enPassantSquare);
        }
        public static Position ParseFEN(string FENString)
        {
            return ParseFEN(FENString.Split(' '));
        }
        #endregion
    }
}
