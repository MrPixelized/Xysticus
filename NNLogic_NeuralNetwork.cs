using System;
using Chess;

namespace NNLogic
{
    class NeuralNetwork
    {
        public float[,] weights;
        readonly int hiddenLayerCount;
        readonly int inputNodeCount;
        readonly int hiddenNodeCount;
        readonly int outputNodeCount;

        public NeuralNetwork(int hiddenLayerCount, int inputNodeCount, int hiddenNodeCount, int outputNodeCount, float[,] weights) {
            this.hiddenLayerCount = hiddenLayerCount;
            this.inputNodeCount = inputNodeCount;
            this.hiddenNodeCount = hiddenNodeCount;
            this.outputNodeCount = outputNodeCount;
            this.weights = weights;
        }			
        public float[] CalculateResult()
        {
            float[] output = { 0.1f };
            return output;
        }
        public float[] _advance(float[] activations, int layer)
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
                    nextActivations[i] += weights[layer, i * (previousLayerNodeCount + 1) + j] * activations[j];
                }
                //Add the bias
                nextActivations[i] += weights[layer, (i + 1) * (previousLayerNodeCount + 1) - 1];
            }
            return nextActivations;
        }
        private float _activation(float activation)
        {
            return (activation > 0) ? activation : 0;
        }
    }
}
