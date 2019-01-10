using NNLogic;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Text;
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
        public int engineDepth;
        public bool storeGame;

        public Game(Position startingPosition = null)
        {
            currentPosition = startingPosition ?? new Position();
            if (storeGame) moveHistory = new List<string>();
        }        
        public void Play()
        {
            Move bestMove;
            List<(Position, Move)> nextPositionMoveTupleList = currentPosition.GeneratePositions();
            while (result == null)
            {
                if (currentPosition.toMove == WHITE)
                {
                    // Make the white player find a move and apply it to the position.
                    bestMove = Engine.FindBestMove(whitePlayer.EvaluatePosition, currentPosition, 
                        nextPositionMoveTupleList, engineDepth, -2.0f, 2.0f).Item1;
                    currentPosition = currentPosition.MakeMove(bestMove);
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
                    bestMove = Engine.FindBestMove(blackPlayer.EvaluatePosition, currentPosition, 
                        nextPositionMoveTupleList, engineDepth, -2.0f, 2.0f).Item1;
                    currentPosition = currentPosition.MakeMove(bestMove);
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
                if (storeGame) moveHistory.Add(UCIProtocol.MoveToUCINotation(bestMove) + " ");
            }
        }
        public override string ToString()
        {
            if (storeGame)
            {
                StringBuilder sb = new StringBuilder();
                foreach (string net in moveHistory)
                {
                    sb.Append(net + " ");
                }
                return sb.ToString();
            }
            else
            {
                return "Game not stored.";
            }
        }
    }
}
