using System;
using System.Text;
using Chess;

namespace NNLogic
{
    public class NeuralNetwork
    {
        public float[][] weights;
        public float fitness;
        public readonly int hiddenLayerCount;
        public readonly int inputNodeCount;
        public readonly int hiddenNodeCount;
        public readonly int outputNodeCount;

        public NeuralNetwork(int hiddenLayerCount, int inputNodeCount, int hiddenNodeCount, int outputNodeCount, float[][] weights) {
            this.hiddenLayerCount = hiddenLayerCount;
            this.inputNodeCount = inputNodeCount;
            this.hiddenNodeCount = hiddenNodeCount;
            this.outputNodeCount = outputNodeCount;
            this.weights = weights;
        }			
        public float[] CalculateResult(float[] activations)
        {
            float[] currentActivations = activations;
            for (int i = 0; i <= hiddenLayerCount; i++)
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
                    // The index of connection from j to i is: i * (number of nodes in the previous layer) + j
                    nextActivations[i] += weights[layer][i * (previousLayerNodeCount + 1) + j] * activations[j];
                }
                // Add the bias
                nextActivations[i] += weights[layer][(i + 1) * (previousLayerNodeCount + 1) - 1];
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
    }
}
