using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Chess;

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
            // The selection process all networks have to go through to determine if they can reproduce.
            // Status: finished, but has unfinished dependencies.
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
                // Add the networks to the groups, looping at maxGroupSize.
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
            // Creates a full-sized population from the organisms that made it through selection.
            // Status: finished.
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
            // Status: just a placeholder, not working at the moment.
            
            for (int i = 0; i < nets.Count; i++)
            {
                for (int j = i + 1; j < nets.Count; j++)
                {
                    // Let nets[i] play against nets[j].
                    Game game;
                    if (i % 2 == j % 2)
                    {
                        game = new Game
                        {
                            whitePlayer = nets[i],
                            blackPlayer = nets[j]
                        };
                        game.Play();
                        nets[i].score += (float)game.result;
                        nets[j].score += 1.0f - (float)game.result;
                    }
                    else
                    {
                        game = new Game
                        {
                            whitePlayer = nets[j],
                            blackPlayer = nets[i]
                        };
                        game.Play();
                        nets[j].score += (float)game.result;
                        nets[i].score += 1.0f - (float)(game.result);
                    }
                    nets[i].games++; nets[j].games++;
                    Console.WriteLine(game.result);
                    Console.WriteLine("nets[i] now has a score of {0}, nets[j] {1}", nets[i].score, nets[j].score);
                }
            }
            foreach (NeuralNetwork net in nets)
            {
                net.totalScore += net.score;
                net.totalGames += net.games;
            }
            nets.Sort((x, y) => y.score.CompareTo(x.score));
            return nets.Take((int)Math.Ceiling(selectionFactor * (nets.Count))).ToList();
        }

        private List<NeuralNetwork> _pickNets(float fitnessSum)
        {
            // Returns a set number of organisms that are used to reproduce a single organism
            // Status: finished.
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
            // Takes a set number of networks and randomly combines them into one.
            // Status: just a (working) prototype, could become much more complex.
            int numberOfNets = netsToCombine.Count();
            Random random = new Random();
            float[][] newWeights = netsToCombine[0].weights.Select(s => s.ToArray()).ToArray();
            newWeights[0][0] = 1.01f;
            for (int i = 0; i < netsToCombine[0].weights.Length; i++)
            {
                for (int j = 0; j < netsToCombine[0].weights[i].Length; j++)
                {
                    newWeights[i][j] = netsToCombine[random.Next(0, numberOfNets)].weights[i][j];
                }
            }
            return new NeuralNetwork(netsToCombine[0].hiddenLayerCount, netsToCombine[0].inputNodeCount, netsToCombine[0].hiddenNodeCount, netsToCombine[0].outputNodeCount, newWeights);
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (NeuralNetwork net in population)
            {
                sb.Append(net.ToString());
            }
            return sb.ToString();
        }
    }
}
