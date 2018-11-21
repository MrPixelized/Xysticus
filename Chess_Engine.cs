using System;
using System.Collections.Generic;
using System.Linq;
using NNLogic;
using static Chess.Constants;

namespace Chess
{
    public static class Engine
    {
        public static (Move, float) FindBestMove(ref NeuralNetwork evaluationFunction, Position currentPosition, List<(Position, Move)> positionMoveTupleList, int depth, float alpha, float beta)
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
                    // If the white king wants to castle, there is a couple things we need to check first.
                    {
                        if (!lastMoveWasLegal || currentPosition.IsCheck()) continue;
                    }
                    lastMoveWasLegal = false;
                    // Generate another layer of positions
                    // When in a leaf node, it's still important to do this because this position might be illegal.
                    nextPositionMoveTupleList = position.GeneratePositions();
                    if (nextPositionMoveTupleList.Count == 0)
                    {
                        // After the most recent legal move, there are no pseudo-legal moves and the game has ended.
                        moveEvaluationTupleList.Add((move, evaluationFunction.EvaluatePosition(position)));
                        lastMoveWasLegal = true;
                        }
                    else if (nextPositionMoveTupleList.Last().Item1 != null)
                    // The most recent pseudo-legal move does not allow a king to be captured. 
                    {
                        lastMoveWasLegal = true;
                        if (depth == 0)
                        {
                            moveEvaluationTupleList.Add((move, evaluationFunction.EvaluatePosition(position)));
                        }
                        else
                        {
                            moveEvaluationTupleList.Add((move, FindBestMove(ref evaluationFunction, position, nextPositionMoveTupleList, depth - 1, alpha, beta).Item2));
                        }
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
                    // This only occurs when we're in a position where there are pseudo-legal moves, but no legal moves.
                    // In this case, it's safe to return a random move and the evaluation for the final position.
                    return (positionMoveTupleList.Last().Item2, evaluationFunction.EvaluatePosition(currentPosition));
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
                    // If the black king wants to castle, there is a couple things we need to check first.
                    {
                        if (!lastMoveWasLegal || currentPosition.IsCheck()) continue;
                    }
                    // Generate another layer of positions
                    // When in a leaf node, it's still important to do this because this position might be illegal.
                    nextPositionMoveTupleList = position.GeneratePositions();
                    if (nextPositionMoveTupleList.Count == 0)
                    {
                        // After the most recent legal move, there are no pseudo-legal moves and the game has ended. 
                        moveEvaluationTupleList.Add((move, evaluationFunction.EvaluatePosition(position)));
                    }
                    else if (nextPositionMoveTupleList.Last().Item1 != null)
                    // The pseudo-legal move we're investigating does not allow a king to be captured. 
                    {
                        if (depth == 0)
                        {
                            moveEvaluationTupleList.Add((move, evaluationFunction.EvaluatePosition(position)));
                        }
                        else
                        {
                            moveEvaluationTupleList.Add((move, FindBestMove(ref evaluationFunction, position, nextPositionMoveTupleList, depth - 1, alpha, beta).Item2));
                        }                        
                    }
                    // MINIMAX
                    if (moveEvaluationTupleList.Count != 0)
                    {
                        bestEvaluation = Math.Min(bestEvaluation, moveEvaluationTupleList.Last().Item2);
                        beta = Math.Min(bestEvaluation, beta);
                        if (beta <= alpha) break;
                    }
                }
                if (moveEvaluationTupleList.Count == 0)
                {
                    // This only occurs when we're in a position where there are pseudo-legal moves, but no legal moves.
                    // In this case, it's safe to return a random move and the evaluation for the final position.
                    return (positionMoveTupleList.Last().Item2, evaluationFunction.EvaluatePosition(currentPosition));
                }
                moveEvaluationTupleList.Sort((x, y) => x.Item2.CompareTo(y.Item2));
            }
            return moveEvaluationTupleList[0];
        }
        public static (Move, float) FindBestMove(ref NeuralNetwork evaluationFunction, Position currentPosition, int depth)
        {
            List<(Position, Move)> nextPositionMoveTupleList = currentPosition.GeneratePositions();
            return FindBestMove(ref evaluationFunction, currentPosition, nextPositionMoveTupleList, depth, -2.0f, 2.0f);
        }
    }
}
 
