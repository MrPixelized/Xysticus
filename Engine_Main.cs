using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using Chess;
using Interface;

namespace ChessEngine
{
    public class Engine
    {
        private static Game game;
        //private Thread findBestMoveThread;

        public Engine()
        {
            //findBestMoveThread = new Thread(FindBestMove);
            if (game == null)
            {
                game = new Game();
            }
        }

        public void NewGame()
        {
            game = new Game();
        }

        public void SetPosition(string[] inputStringArray)
        {
            Position position;
            int firstMove;

            if (inputStringArray[0] == "fen")
            {
                List<String> FENStringList = new List<String>();
                for (int i = 1; i <= 6; i++)
                {
                    FENStringList.Add(inputStringArray[i]);
                }
                position = FENParser.ParseFEN(FENStringList.ToArray());
                firstMove = 8;
            }
            else
            {
                position = new Position();
                firstMove = 2;
            }

            Move move;
            string moveString;

            for (int i = firstMove; i < inputStringArray.Length; i++)
            {
                moveString = inputStringArray[i];
                move = new Move(Interface.Constants.COORDINATE_TRANSFORMATION(moveString[0]), 8 - (int)Char.GetNumericValue(moveString[1]), Interface.Constants.COORDINATE_TRANSFORMATION(moveString[2]), 8 - (int)Char.GetNumericValue(moveString[3]));
                position = position.MakeMove(move);
            }
            game.currentPosition = position;
        }

        public void SetPosition(Position currentPosition)
        {
            game.currentPosition = currentPosition;
        }

        public Move FindBestMove()
        {
            List<(Position, Move)> moveList = game.currentPosition.GeneratePositions().ToList();
            Random rnd = new Random();
            Move bestMove = moveList[rnd.Next(1, moveList.Count())].Item2;
            //Console.WriteLine(String.Format("The absolute best move in this position is {0}, {1} to {2}, {3}", bestMove.fromX, bestMove.fromY, bestMove.toX, bestMove.toY));
            return bestMove;
        }

        public void StopFindingBestMove()
        {

        }

        public Position GetPosition()
        {
            return game.currentPosition;
        }
    }
}
