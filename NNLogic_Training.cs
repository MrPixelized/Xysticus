using System;
using System.Collections.Generic;
using System.Linq;

namespace NNLogic
{
    public class Training
    {
        public List<NeuralNetwork> population;
        int generation;
        readonly int reproductionOrganismCount;
        readonly int maxGroupSize;
        readonly float selectionFactor;
        readonly int populationSizeBeforeTrimming;
        readonly int populationSizeAfterTrimming;
        Random random;

        public Training(List<NeuralNetwork> population, int reproductionOrganismCount, int maxGroupSize, float selectionFactor, int populationSizeBeforeTrimming, int populationSizeAfterTrimming)
        {
            this.population = population;
            this.reproductionOrganismCount = reproductionOrganismCount;
            this.maxGroupSize = maxGroupSize;
            this.selectionFactor = selectionFactor;
            this.populationSizeAfterTrimming = populationSizeAfterTrimming;
            this.populationSizeBeforeTrimming = populationSizeBeforeTrimming;
            random = new Random();
        }

        public void Train()
        {
            while (true)
            {
                generation += 1;
                RemoveNets();
                /* Rate the fitnesses of the leftover networks
                Recombine the networks into new ones based on their fitness
                Mutatie yadadaya
                Recombination yadadayadayaddaadaya
                Put these new networks and(some of) the ones from the previous population of size F generated at 1. in a new population, repeat from 1.
                */
                GenerateNextGeneration();
            }
        }

        public void RemoveNets()
        {
            List<List<NeuralNetwork>> groupList;
            while (population.Count > populationSizeAfterTrimming)
            {
                // Step 1: Divide the population into groups of ~equal size
                groupList = new List<List<NeuralNetwork>>();
                // Initialize groupList
                int groupCount = (int)Math.Ceiling((double)population.Count / maxGroupSize);
                // Add the right number of NeuralNetwork groups to the group list.
                for (int i = 0; i < groupCount; i++)
                {
                    groupList.Add(new List<NeuralNetwork>());
                }
                // Add the networks to the groups
                for (int i = 0; i < population.Count; i++)
                {
                    groupList[i % maxGroupSize].Add(population[i]);
                }
                // Select networks to continue to the next round
                List<NeuralNetwork> nextPopulation = new List<NeuralNetwork>();
                for (int i = 0; i < groupList.Count; i++)
                {
                    nextPopulation.AddRange(RunRoundRobin(groupList[i]));
                }
                population = nextPopulation;
            }
        }

        public void GenerateNextGeneration()
        {
            float fitnessSum = 0;
            foreach (NeuralNetwork net in population)
            {
                fitnessSum += net.fitness;
            }
            while (population.Count < populationSizeBeforeTrimming)
            {
                // Generate a new individual
                List<NeuralNetwork> recombinationList = _pickNets(fitnessSum);
                population.Add(_crossover(recombinationList));
            }
        }

        public List<NeuralNetwork> RunRoundRobin(List<NeuralNetwork> nets)
        {
            // Return the strongest ceil(selectionFactor * nets.Count) nets
            return (List<NeuralNetwork>)nets.Take((int)Math.Ceiling(selectionFactor * (nets.Count)));
        }

        private List<NeuralNetwork> _pickNets(float fitnessSum)
        {
            // Returns a set number of organisms that are used to reproduce a single organism
            List<NeuralNetwork> recombinationList = new List<NeuralNetwork>();
            for (int i = 0; i < reproductionOrganismCount; i++)
            {
                float fitnessIndex = (float)random.NextDouble() * fitnessSum;
                float temporaryFitnessSum = 0;
                for (int j = 0; j < population.Count; j++)
                {
                    temporaryFitnessSum += population[j].fitness;
                    if (fitnessIndex <= temporaryFitnessSum)
                    {
                        recombinationList.Add(population[j]);
                        break;
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
