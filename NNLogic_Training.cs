using System;
using System.Collections.Generic;
using System.Linq;

namespace NNLogic
{
    public class Training
    {
        public NeuralNetwork[] population;
        public float[] fitnesses;
        int generation;
        int reproductionOrganismCount;
        Random random;

        public Training(NeuralNetwork[] population, float[] fitnesses, int reproductionOrganismCount)
        {
            this.population = population;
            this.fitnesses = fitnesses;
            this.reproductionOrganismCount = reproductionOrganismCount;
            random = new Random();
        }

        public void Train()
        {
            // RateNets() - the function that is responsible for assigning fitnesses to the nets.
            // GenerateNextGeneration() - the function that is responsible for making the next generation.
            // Then just loop it, and we're done :).
        }

        public void RateNets()
        {

        }

        public void GenerateNextGeneration()
        {
            float fitnessSum = fitnesses.Sum();

            List<NeuralNetwork> nextPopulation = new List<NeuralNetwork>();
            for (int i = 0; i < population.Length; i++)
            {
                // Generate a new individual
                List<NeuralNetwork> recombinationList = _pickNets(fitnessSum);
                nextPopulation.Add(_crossover(recombinationList));
            }
            population = nextPopulation.ToArray();
        }

        private List<NeuralNetwork> _pickNets(float fitnessSum)
        {
            List<NeuralNetwork> recombinationList = new List<NeuralNetwork>();
            for (int i = 0; i < reproductionOrganismCount; i++)
            {
                float fitnessIndex = (float)random.NextDouble() * fitnessSum;
                float temporaryFitnessSum = 0;
                for (int j = 0; j < population.Length; j++)
                {
                    temporaryFitnessSum += fitnesses[j];
                    if (fitnessSum <= temporaryFitnessSum)
                    {
                        recombinationList.Add(population[j]);
                    }
                }
            }
            return recombinationList;
        }

        private NeuralNetwork _crossover(List<NeuralNetwork> netsToCombine)
        {
            return netsToCombine[0];
        }
    }
}
