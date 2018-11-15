using NNLogic;
using System.Collections.Generic;
using System.Linq;
using Interface;
using static Chess.Constants;

namespace Chess
{
    public class Game
    {
        public Position currentPosition;
        public List<string> moveHistory;
        public NeuralNetwork whitePlayer;
        public NeuralNetwork blackPlayer;
        public bool isCheck;
        public float? result;

        public Game(Position startingPosition = null)
        {
            currentPosition = startingPosition ?? new Position();
            moveHistory = new List<string>();
        }        
        public void Play()
        {
            (Move, float) moveEvaluationTuple;
            List<(Position, Move)> nextPositionMoveTupleList = currentPosition.GeneratePositions();
            while (result == null)
            {
                if (currentPosition.toMove == WHITE)
                {
                    // Make the white player find a move and apply it to the position.
                    moveEvaluationTuple = Engine.FindBestMove(ref whitePlayer, currentPosition, nextPositionMoveTupleList, 2, -2.0f, 2.0f);
                    currentPosition = currentPosition.MakeMove(moveEvaluationTuple.Item1);
                    if (currentPosition.fiftyMoveProximity >= 100)
                    {
                        result = DRAW;
                    }
                    else
                    {
                        nextPositionMoveTupleList = currentPosition.GeneratePositions();
                        if (nextPositionMoveTupleList.Count == 0)
                        {
                            // White just played a move which doesn't give black any pseudo-legal moves.
                            isCheck = currentPosition.IsCheck();
                            result = isCheck ? WHITE_WIN : DRAW;
                        }
                        else if (nextPositionMoveTupleList.Last().Item1 == null)
                        {
                            // White just played an illegal move (which only happens when there are no legal moves).
                            result = isCheck ? BLACK_WIN : DRAW;
                        }
                        isCheck = currentPosition.IsCheck();
                    }
                }
                else
                {
                    // Make the black player find a move and apply it to the position.
                    moveEvaluationTuple = Engine.FindBestMove(ref blackPlayer, currentPosition, nextPositionMoveTupleList, 2, -2.0f, 2.0f);
                    currentPosition = currentPosition.MakeMove(moveEvaluationTuple.Item1);
                    if (currentPosition.fiftyMoveProximity >= 100)
                    {
                        result = DRAW;
                    }
                    else
                    {
                        nextPositionMoveTupleList = currentPosition.GeneratePositions();
                        if (nextPositionMoveTupleList.Count == 0)
                        {
                            // Black just played a move which doesn't give white any pseudo-legal moves.
                            isCheck = currentPosition.IsCheck();
                            result = isCheck ? BLACK_WIN : DRAW;
                        }
                        if (nextPositionMoveTupleList.Last().Item1 == null)
                        {
                            // Black just played an illegal move (which only happens when there are no legal moves).
                            result = isCheck ? WHITE_WIN : DRAW;
                        }
                        isCheck = currentPosition.IsCheck();
                    }
                }
                moveHistory.Add(UCIProtocol.MoveToUCINotation(moveEvaluationTuple.Item1) + " ");
            }
            System.IO.File.WriteAllLines("movehistory.pgn", moveHistory);
        }
    }
}
