using System;
using System.Collections.Generic;
using System.Linq;
using static Chess.Constants;

namespace Chess
{
    public class Engine
    {
        //private Thread findBestMoveThread;
        readonly Random evaluationFunction;

        public Engine()
        {
            evaluationFunction = new Random();
            //findBestMoveThread = new Thread(FindBestMove);
        }

        public void NewGame()
        {
            //Clear all data saved about the game
        }

        public (Move, float) FindBestMove(Position currentPosition, int depth, float alpha, float beta)
        {
            /*
            List<(Position, Move)> moveList = currentPosition.GeneratePositions().ToList();
            Random rnd = new Random();
            Move bestMove = moveList[rnd.Next(1, moveList.Count())].Item2;
            return bestMove;
            */

            List<(Move, float)> moveEvaluationTupleList = new List<(Move, float)>();
            float bestEvaluation;
            // Maximizing player
            if (currentPosition.toMove == WHITE)
            {
                bestEvaluation = -100;
                foreach ((Position position, Move move) in currentPosition.GeneratePositions())
                {
                    //Console.WriteLine(String.Format("{0}White here, now looking into {1}, {2} to {3}, {4}", new String('\t', 1 - depth), move.fromX, move.fromY, move.toX, move.toY));
                    // If a king can be captured in this position, make sure that the engine never chooses this position
                    if (position == null)
                    {
                        return (move, 100);
                    }

                    // If the requested depth has not yet been reached, generate another layer of positions
                    if (depth == 0)
                    {
                        moveEvaluationTupleList.Add((move, (float)evaluationFunction.NextDouble()));
                    }
                    else
                    {
                        moveEvaluationTupleList.Add((move, FindBestMove(position, depth - 1, alpha, beta).Item2));
                        //Console.WriteLine(String.Format("{0}The value we're adding to {1}, {2} to {3}, {4} is {5}", new String('\t', 1 - depth), move.fromX, move.fromY, move.toX, move.toY, moveEvaluationTupleList.Last().Item2));
                    }
                    // MINIMAX
                    bestEvaluation = Math.Max(bestEvaluation, moveEvaluationTupleList.Last().Item2);
                    alpha = Math.Max(bestEvaluation, alpha);
                    if (beta <= alpha)
                    {
                        //Console.WriteLine("beta <= alpha, so we're breaking.");
                        break;
                    }
                }
                moveEvaluationTupleList.Sort((x, y) => y.Item2.CompareTo(x.Item2));
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
                        return (move, -100);
                    }

                    // If the requested depth has not yet been reached, generate another layer of positions
                    if (depth == 0)
                    {
                        moveEvaluationTupleList.Add((move, (float)evaluationFunction.NextDouble()));
                        //Console.WriteLine(String.Format("{0}The random value we're adding to {1}, {2} to {3}, {4} is {5}", new String('\t', 1 - depth), move.fromX, move.fromY, move.toX, move.toY, moveEvaluationTupleList.Last().Item2));
                    }
                    else
                    {
                        moveEvaluationTupleList.Add((move, FindBestMove(position, depth - 1, alpha, beta).Item2));
                    }
                    // MINIMAX
                    bestEvaluation = Math.Min(bestEvaluation, moveEvaluationTupleList.Last().Item2);
                    beta = Math.Min(bestEvaluation, beta);
                    if (beta <= alpha) break;
                }
                moveEvaluationTupleList.Sort((x, y) => x.Item2.CompareTo(y.Item2));
            }
            foreach ((Move move, float evaluation) in moveEvaluationTupleList)
            {
                //Console.WriteLine(String.Format("{0}The move {1},{2} to {3},{4} has an evaluation of {5}", new String('\t', 1 - depth), move.fromX, move.fromY, move.toX, move.toY, evaluation));
            }
            (Move, float) bestMoveEvaluationTuple = moveEvaluationTupleList[0];
            return (bestMoveEvaluationTuple);
        }
    }
}
 
