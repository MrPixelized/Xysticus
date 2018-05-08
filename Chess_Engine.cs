using System;
using System.Collections.Generic;
using System.Linq;

namespace Chess
{
    public class Engine
    {
        //private Thread findBestMoveThread;

        public Engine()
        {
            //findBestMoveThread = new Thread(FindBestMove);
        }

        public void NewGame()
        {
            //Clear all data saved about the game
        }

        public Move FindBestMove(Position currentPosition, int depth, float alpha, float beta)
        {
            List<(Position, Move)> moveList = currentPosition.GeneratePositions().ToList();
            Random rnd = new Random();
            Move bestMove = moveList[rnd.Next(1, moveList.Count())].Item2;
            return bestMove;

            /*
            float bestEvaluation;
            float evaluation;
            if (depth <= 0)
            {
                return (float)new Random().NextDouble();
            }
            // Maximizing player
            if (currentPosition.toMove == 1)
            {
                bestEvaluation = -100;
                foreach ((Position position, Move move) in currentPosition.GeneratePositions())
                {
                    // If a king can be captured in this position, make sure that the engine never chooses this position
                    if (position == null)
                    {
                        return -3;
                    }

                    // If the requested depth has not yet been reached, generate another layer of positions
                    evaluation = FindBestMove(position, depth - 1, alpha, beta);

                    // MINIMAX
                    bestEvaluation = Math.Max(bestEvaluation, evaluation);
                    alpha = Math.Max(bestEvaluation, beta);
                    if (beta <= alpha) break;
                }
            }

            // Minimizing player
            else
            {
                bestEvaluation = 100;
                foreach ((Position position, Move move) in currentPosition.GeneratePositions())
                {
                    // If a king can be captured in this position, make sure that the engine never chooses this position
                    if (position == null)
                    {
                        return 3;
                    }

                    // If the requested depth has not yet been reached, generate another layer of positions
                    evaluation = FindBestMove(position, depth - 1, alpha, beta);

                // MINIMAX
                bestEvaluation = Math.Min(bestEvaluation, evaluation);
                beta = Math.Min(bestEvaluation, beta);
                if (beta <= alpha) break;
                }
            }
            return bestEvaluation;
            /*List<(Position, Move)> moveList = position.GeneratePositions().ToList();
            Random rnd = new Random();
            Move bestMove = moveList[rnd.Next(1, moveList.Count())].Item2;
            //Console.WriteLine(String.Format("The absolute best move in this position is {0}, {1} to {2}, {3}", bestMove.fromX, bestMove.fromY, bestMove.toX, bestMove.toY));
            return bestMove;
            */
        }
    }
}
