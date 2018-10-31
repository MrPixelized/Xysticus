using NNLogic;
using System;

namespace Chess
{
    public class Game
    {
        public Position currentPosition;
        public NeuralNetwork whitePlayer;
        public NeuralNetwork blackPlayer;

        public Game()
        {
            currentPosition = new Position();
        }        
        public void Play()
        {
            (Move, float) moveEvaluationTuple;
            while (true)
            {
                // Make the white player find a move and apply it to the position.
                moveEvaluationTuple = Engine.FindBestMove(ref whitePlayer, currentPosition, 2);
                currentPosition = currentPosition.MakeMove(moveEvaluationTuple.Item1);
                // Make the black player find a move and apply it to the position.
                moveEvaluationTuple = Engine.FindBestMove(ref blackPlayer, currentPosition, 2);
                currentPosition = currentPosition.MakeMove(moveEvaluationTuple.Item1);
            }
        }
    }
}
