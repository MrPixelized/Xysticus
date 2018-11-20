using System;
using System.Text;
using System.Linq;
using Chess;

namespace NNLogic
{
    public class NeuralNetwork
    {
        public float[][] weights;
        public float fitness;
        public float score;
        public int games;
        public float totalScore;
        public int totalGames;
        public readonly int hiddenLayerCount;
        public readonly int inputNodeCount;
        public readonly int hiddenNodeCount;
        public readonly int outputNodeCount;

        public NeuralNetwork(int hiddenLayerCount, int inputNodeCount, int hiddenNodeCount, int outputNodeCount, float[][] weights)
        {
            this.hiddenLayerCount = hiddenLayerCount;
            this.inputNodeCount = inputNodeCount;
            this.hiddenNodeCount = hiddenNodeCount;
            this.outputNodeCount = outputNodeCount;
            this.weights = weights;
        }
        public NeuralNetwork(int hiddenLayerCount, int inputNodeCount, int hiddenNodeCount, int outputNodeCount)
        {
            this.hiddenLayerCount = hiddenLayerCount;
            this.inputNodeCount = inputNodeCount;
            this.hiddenNodeCount = hiddenNodeCount;
            this.outputNodeCount = outputNodeCount;
            weights = new float[hiddenLayerCount + 1][];
            _initweights();
        }
        private void _initweights()
        {
            Random random = new Random();
            weights[0] = new float[(inputNodeCount + 1) * hiddenNodeCount];
            for (int i = 0; i < (inputNodeCount + 1) * hiddenNodeCount; i++)
            {
                weights[0][i] = _generateweight(ref random);

            }
            for (int i = 0; i < hiddenLayerCount - 1; i++)
            {
                weights[i + 1] = new float[(hiddenNodeCount + 1) * hiddenNodeCount];
                for (int j = 0; j < (hiddenNodeCount + 1) * hiddenNodeCount; j++)
                {
                    weights[i + 1][j] = _generateweight(ref random);
                }
            }
            weights[hiddenLayerCount] = new float[(hiddenNodeCount + 1) * outputNodeCount];
            for (int i = 0; i < (hiddenNodeCount + 1) * outputNodeCount; i++)
            {
                weights[hiddenLayerCount][i] = _generateweight(ref random);
            }
        }
        private float _generateweight(ref Random random)
        {
            float mean = 0;
            float stdDev = 1;
            double u1 = 1.0 - random.NextDouble();
            double u2 = 1.0 - random.NextDouble();
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) *
                         Math.Sin(2.0 * Math.PI * u2);
            double randNormal =
                         mean + stdDev * randStdNormal;
            return (float)randNormal;
        }
        public float EvaluatePosition(Position position)
        {
            // Turns a Chess.Position into input for a neural network.
            // Status: finished.
            float[] networkInput = new float[12 * 64 + 16 + 4 + 1 + 1];
            /* Distribution of nodes:
             * 12 * 64 possible piece positions
             * 16 possible en passant squares
             * 4 castling rights
             * 1 side to move
             * 1 fifty-move rule
             */
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    int pieceNumberType = position.board[i, j];
                    if (pieceNumberType != 0)
                    {
                        // position.board[i, j] contains a piece
                        // Since there is a gap between -1 and 1, we may have to add 5 or 6 to the piece number type to obtain the appropriate input index
                        int inputIndex = pieceNumberType > 0 ? pieceNumberType + 5 : pieceNumberType + 6;
                        // Set the appropriate input node to 1
                        networkInput[inputIndex * 64 + 8 * i + j] = 1;
                    }
                }
            }
            if (position.enPassantSquare.Item1 != -1)
            {
                // There is an en passant square on the board
                int inputIndex = position.enPassantSquare.Item1 * 2;
                if (position.enPassantSquare.Item2 == 5) inputIndex += 1;
                // Set the appropriate input node to 1
                networkInput[12 * 64 + inputIndex] = 1;
            }
            for (int i = 0; i < 4; i++)
            {
                // Set the castling rights
                networkInput[12 * 64 + 16 + i] = position.castlingRights[i] ? 1 : 0;
            }
            // Set the side to move
            networkInput[12 * 64 + 16 + 4] = position.toMove;
            // Set the fifty move proximity
            networkInput[12 * 64 + 16 + 4 + 1] = position.fiftyMoveProximity;
            return _sigmoid(CalculateResult(networkInput)[0]);
        }
        public float[] CalculateResult(float[] activations)
        {
            float[] currentActivations = activations;
            for (int i = 1; i < hiddenLayerCount + 1; i++)
            {
                currentActivations = _advance(currentActivations, i);
            }
            return currentActivations;
        }
        private float[] _advance(float[] activations, int layer)
        {
            // The node count of this layer and that of the previous is dependent on the layer we are calculating the activations of.
            int thisLayerNodeCount = (layer > hiddenLayerCount) ? outputNodeCount : hiddenNodeCount;
            int previousLayerNodeCount = (layer == 1) ? inputNodeCount : hiddenNodeCount;
            float[] nextActivations = new float[thisLayerNodeCount];

            for (int i = 0; i < thisLayerNodeCount; i++)
            {
                // i represents the index of the neuron of which we are currently calculating the activation.
                // Each neuron has hiddenNodeCount + 1 (for the bias) inputs, unless layer = 1, in which case it has inputNodeCount + 1 inputs.
                nextActivations[i] = 0;
                for (int j = 0; j < previousLayerNodeCount; j++)
                {
                    // j represents the index of the neuron of which we are currently using the activation.
                    // The index of connection from j to i is: i * (number of nodes in the previous layer + 1) + j
                    nextActivations[i] += weights[layer - 1][i * (previousLayerNodeCount + 1) + j] * activations[j];
                }
                // Add the bias
                nextActivations[i] += weights[layer - 1][(i + 1) * (previousLayerNodeCount + 1) - 1];
                // Apply the activation function
                nextActivations[i] = _activation(nextActivations[i]);
            }
            return nextActivations;
        }
        private float _activation(float activation)
        {
            return Math.Max(activation, 0);
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append('{');
            for (int i = 0; i < hiddenLayerCount + 1; i++)
            {
                sb.Append('{');
                for (int j = 0; j < weights[i].Length; j++)
                {
                    sb.Append(weights[i][j].ToString());
                    if (!(j == weights[i].Length - 1))
                    {
                        sb.Append(' ');
                    }
                }
                sb.Append('}');
            }
            sb.Append('}');
            return sb.ToString();
        }
        private float _sigmoid(double x)
        {
            return (float)(1 / (1 + Math.Pow(Math.E, -x)));
        }
    }
}
