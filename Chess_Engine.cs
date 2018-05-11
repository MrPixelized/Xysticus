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

        public (Move, float) FindBestMove(Position currentPosition, List<(Position, Move)> positionMoveTupleList, int depth, float alpha, float beta)
        {
            List<(Move, float)> moveEvaluationTupleList = new List<(Move, float)>();
            List<(Position, Move)> nextPositionMoveTupleList;
            float bestEvaluation;
            bool lastMoveWasLegal = false;
            // Maximizing player
            if (currentPosition.toMove == WHITE)
            {
                bestEvaluation = -100;
                foreach ((Position position, Move move) in positionMoveTupleList)
                {
                    if (move.pieceToMove == WHITE_KING && (move.toX - move.fromX == 2 || move.toX - move.fromX == -2))
                    //If the white king wants to castle, there is a couple things we need to check first.
                    {
                        if (!(lastMoveWasLegal && !currentPosition.IsInCheck()))
                        {
                            continue;
                        }
                    }

                    lastMoveWasLegal = false;
                    //Console.WriteLine(String.Format("{0}White here, now looking into {1}, {2} to {3}, {4}", new String('\t', 1 - depth), move.fromX, move.fromY, move.toX, move.toY));
                    //Generate positions for the second generation of child nodes from here.
                    //This is necessary to get rid of illegal moves in the first generation of child nodes.
                    nextPositionMoveTupleList = position.GeneratePositions();
                    if (nextPositionMoveTupleList.Count == 0)
                    {
                        //After the most recent legal move, there are no pseudo-legal moves and the game has ended.
                        moveEvaluationTupleList.Add((move, (float)evaluationFunction.NextDouble()));
                        lastMoveWasLegal = true;
                        //Interface.ConsoleGraphics.DrawPosition(position);
                        //Console.WriteLine(String.Format("{0}The value we're adding to {1}, {2} to {3}, {4} is {5}", new String('\t', 1 - depth), move.fromX, move.fromY, move.toX, move.toY, moveEvaluationTupleList.Last().Item2));
                    }
                    else if (nextPositionMoveTupleList.Last().Item1 != null)
                    //The most recent pseudo-legal move does not allow a king to be captured. 
                    {
                        lastMoveWasLegal = true;
                        if (depth == 0)
                        {
                            moveEvaluationTupleList.Add((move, (float)evaluationFunction.NextDouble()));
                            //Interface.ConsoleGraphics.DrawPosition(position);
                        }
                        else
                        {
                            moveEvaluationTupleList.Add((move, FindBestMove(position, nextPositionMoveTupleList, depth - 1, alpha, beta).Item2));
                            //Interface.ConsoleGraphics.DrawPosition(position);
                        }
                        //Console.WriteLine(String.Format("{0}The value we're adding to {1}, {2} to {3}, {4} is {5}", new String('\t', 1 - depth), move.fromX, move.fromY, move.toX, move.toY, moveEvaluationTupleList.Last().Item2));
                    }
                    // MINIMAX
                    if (moveEvaluationTupleList.Count != 0)
                    {
                        bestEvaluation = Math.Max(bestEvaluation, moveEvaluationTupleList.Last().Item2);
                        alpha = Math.Max(bestEvaluation, alpha);
                        if (beta <= alpha) break;
                    }
                }
                if (moveEvaluationTupleList.Count == 0)
                {
                    //This only occurs when we're in a position where there are pseudo-legal moves, but no legal moves.
                    //In this case, it's safe to return a random move and the evaluation for the final position.
                    return (positionMoveTupleList.Last().Item2, (float)evaluationFunction.NextDouble());
                }
                moveEvaluationTupleList.Sort((x, y) => y.Item2.CompareTo(x.Item2));
            }
            // Minimizing player
            else
            {
                bestEvaluation = 100;
                foreach ((Position position, Move move) in positionMoveTupleList)
                {
                    if (move.pieceToMove == BLACK_KING && (move.toX - move.fromX == 2 || move.toX - move.fromX == -2))
                    //If the black king wants to castle, there is a couple things we need to check first.
                    {
                        if (!(lastMoveWasLegal && !currentPosition.IsInCheck()))
                        {
                            continue;
                        }
                    }
                    //Console.WriteLine(String.Format("{0}Black here, now looking into {1}, {2} to {3}, {4}", new String('\t', 1 - depth), move.fromX, move.fromY, move.toX, move.toY));

                    // If the requested depth has not yet been reached, generate another layer of positions
                    nextPositionMoveTupleList = position.GeneratePositions();
                    if (nextPositionMoveTupleList.Count == 0)
                    {
                        //After the most recent legal move, there are no pseudo-legal moves and the game has ended. 
                        moveEvaluationTupleList.Add((move, (float)evaluationFunction.NextDouble()));
                        //Interface.ConsoleGraphics.DrawPosition(position);
                        //Console.WriteLine(String.Format("{0}The value we're adding to {1}, {2} to {3}, {4} is {5}", new String('\t', 1 - depth), move.fromX, move.fromY, move.toX, move.toY, moveEvaluationTupleList.Last().Item2));
                    }
                    else if (nextPositionMoveTupleList.Last().Item1 != null)
                    //The pseudo-legal move we're investigating does not allow a king to be captured. 
                    {
                        if (depth == 0)
                        {
                            moveEvaluationTupleList.Add((move, (float)evaluationFunction.NextDouble()));
                            //Interface.ConsoleGraphics.DrawPosition(position);
                        }
                        else
                        {
                            moveEvaluationTupleList.Add((move, FindBestMove(position, nextPositionMoveTupleList, depth - 1, alpha, beta).Item2));
                            //Interface.ConsoleGraphics.DrawPosition(position);
                        }
                        //Console.WriteLine(String.Format("{0}The value we're adding to {1}, {2} to {3}, {4} is {5}", new String('\t', 1 - depth), move.fromX, move.fromY, move.toX, move.toY, moveEvaluationTupleList.Last().Item2));
                    }
                    //MINIMAX
                    if (moveEvaluationTupleList.Count != 0)
                    {
                        bestEvaluation = Math.Min(bestEvaluation, moveEvaluationTupleList.Last().Item2);
                        beta = Math.Min(bestEvaluation, beta);
                        if (beta <= alpha) break;
                    }
                }
                if (moveEvaluationTupleList.Count == 0)
                {
                    //This only occurs when we're in a position where there are pseudo-legal moves, but no legal moves.
                    //In this case, it's safe to return a random move and the evaluation for the final position.
                    return (positionMoveTupleList.Last().Item2, (float)evaluationFunction.NextDouble());
                }
                moveEvaluationTupleList.Sort((x, y) => x.Item2.CompareTo(y.Item2));
            }
            foreach ((Move move, float evaluation) in moveEvaluationTupleList)
            {
                //Console.WriteLine(String.Format("{0}The move {1},{2} to {3},{4} has an evaluation of {5}", new String('\t', 1 - depth), move.fromX, move.fromY, move.toX, move.toY, evaluation));
            }
            return moveEvaluationTupleList[0];
        }
    }
}
 
