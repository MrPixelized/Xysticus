using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Chess;
using Interface;

namespace NNLogic
{
    public class Training
    {
        readonly int reproductionOrganismCount;
        readonly int maxGroupSize;
        readonly float selectionFactor;
        readonly int populationSizeBeforeTrimming;
        readonly int populationSizeAfterTrimming;
        readonly int hiddenLayerCount;
        readonly int inputNodeCount;
        readonly int hiddenNodeCount;
        readonly int outputNodeCount;
        readonly float mutationRate;
        readonly int testFrequency;
        readonly int engineDepth;
        public List<NeuralNetwork> population;
        int generation;
        NeuralNetwork previousBest;
        Random random;

        public Training(int reproductionOrganismCount, int maxGroupSize, float selectionFactor,
            int populationSizeBeforeTrimming, int populationSizeAfterTrimming, int hiddenLayerCount,
            int inputNodeCount, int hiddenNodeCount, int outputNodeCount, float mutationRate, int testFrequency,
            int engineDepth, List<NeuralNetwork> population = null)
        {
            this.reproductionOrganismCount = reproductionOrganismCount;
            this.maxGroupSize = maxGroupSize;
            this.selectionFactor = selectionFactor;
            this.populationSizeAfterTrimming = populationSizeAfterTrimming;
            this.populationSizeBeforeTrimming = populationSizeBeforeTrimming;
            this.hiddenLayerCount = hiddenLayerCount;
            this.inputNodeCount = inputNodeCount;
            this.hiddenNodeCount = hiddenNodeCount;
            this.outputNodeCount = outputNodeCount;
            this.mutationRate = mutationRate;
            this.testFrequency = testFrequency;
            this.engineDepth = engineDepth;
            this.population = population ?? InitializePopulation();
            random = new Random();
        }

        public List<NeuralNetwork> InitializePopulation()
        {
            List<NeuralNetwork> initialPopulation = new List<NeuralNetwork>();
            for (int i = 0; i < populationSizeBeforeTrimming; i++)
            {
                initialPopulation.Add(new NeuralNetwork(hiddenLayerCount, inputNodeCount, hiddenNodeCount, outputNodeCount));
            }
            return initialPopulation;
        }

        public void Train()
        {
            previousBest = population[0];
            while (true)
            {
                generation += 1;
                SelectionTournament();
                SetFitnesses();
                SortNets();
                BestToFile();
                if (generation % testFrequency == 0)
                {
                    CompareBestToPrevious();
                }
                GenerateNextGeneration();
                MutatePopulation();
            }
        }

        public void SelectionTournament()
        {
            // The selection process all networks have to go through to determine if they can reproduce
            List<List<NeuralNetwork>> groupList;
            while (population.Count > populationSizeAfterTrimming)
            {
                // Divide the population into groups of ~equal size
                // Initialize groupList
                groupList = new List<List<NeuralNetwork>>();
                int groupCount = (int)Math.Ceiling((double)population.Count / maxGroupSize);
                // Add the right number of NeuralNetwork groups to the group list
                for (int i = 0; i < groupCount; i++)
                {
                    groupList.Add(new List<NeuralNetwork>());
                }
                // Add the networks to the groups, looping at groupCount
                for (int i = 0; i < population.Count; i++)
                {
                    groupList[i % groupCount].Add(population[i]);
                }
                // Select networks to continue to the next round
                List<NeuralNetwork> nextPopulation = new List<NeuralNetwork>();
                for (int i = 0; i < groupList.Count; i++)
                {
                    nextPopulation.AddRange(RunRoundRobin(groupList[i]));
                }
                population = nextPopulation;
                foreach (NeuralNetwork net in population)
                {
                    net.games = 0;
                    net.score = 0;
                }
            }
        }

        public void SetFitnesses()
        {
            foreach (NeuralNetwork net in population)
            {
                net.fitness = net.totalScore / net.totalGames;
            }
        }

        public void SortNets()
        {
            population.Sort((x, y) => y.fitness.CompareTo(x.fitness));
            foreach (NeuralNetwork net in population)
            {
                net.score = 0;
                net.totalScore = 0;
                net.games = 0;
                net.totalGames = 0;
            }
        }

        public void BestToFile()
        {
            System.IO.File.AppendAllText("nethistory.txt", population[0].ToString());
        }

        public void CompareBestToPrevious()
        {
            float score = 0;
            Game game;
            game = new Game
            {
                whitePlayer = population[0],
                blackPlayer = previousBest,
                engineDepth = engineDepth
            };
            game.Play();
            score += (float)game.result;
            game = new Game
            {
                whitePlayer = previousBest,
                blackPlayer = population[0],
                engineDepth = engineDepth
            };
            game.Play();
            score -= (float)game.result;
            string logLine = "Score: " + score.ToString() + "; Generation: " + generation.ToString();
            Console.WriteLine(logLine);
            System.IO.File.AppendAllText("scores.txt", logLine);
            previousBest = population[0];
        }

        public void GenerateNextGeneration()
        {
            // Creates a full-sized population from the organisms that made it through selection
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

        public void MutatePopulation()
        {
            foreach (NeuralNetwork net in population)
            {
                foreach (float[] weightArray in net.weights)
                {
                    for (int i = 0; i < weightArray.Length; i++)
                    {
                        weightArray[i] += (float)(random.NextDouble() * 2 - 1) * mutationRate;
                    }
                }
            }
        }

        public List<NeuralNetwork> RunRoundRobin(List<NeuralNetwork> nets)
        {
            // Return the strongest ceil(selectionFactor * nets.Count) nets
            Console.WriteLine("\nNew round robin:\n");

            foreach (Game game in GeneratePairings(nets))
            {
                game.Play();
                game.whitePlayer.score += (float)game.result;
                game.blackPlayer.score += 1.0f - (float)game.result;
                ConsoleGraphics.WriteResult(game.whitePlayer, game.blackPlayer, (float)game.result);
                game.whitePlayer.games++; game.blackPlayer.games++;
            }
            
            foreach (NeuralNetwork net in nets)
            {
                // Update the score of the individuals in the tournament with the scores achieved this round
                net.totalScore += net.score;
                net.totalGames += net.games;
            }
            nets = nets.OrderBy(s => s.score).ThenBy(s => s.totalScore).ToList();
            nets.Reverse();
            return nets.Take((int)Math.Ceiling(selectionFactor * (nets.Count))).ToList();
        }

        public IEnumerable<Game> GeneratePairings(List<NeuralNetwork> nets)
        {
            for (int i = 0; i < nets.Count; i++)
            {
                for (int j = i + 1; j < nets.Count; j++)
                {
                    Game game;
                    if (i % 2 == j % 2)
                    {
                        game = new Game
                        {
                            whitePlayer = nets[i],
                            blackPlayer = nets[j],
                            engineDepth = engineDepth
                        };
                    }
                    else
                    {
                        game = new Game
                        {
                            whitePlayer = nets[j],
                            blackPlayer = nets[i],
                            engineDepth = engineDepth
                        };
                    }
                    yield return game;
                }
            }
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
            // Takes a set number of networks and randomly combines them into one
            int numberOfNets = netsToCombine.Count();
            float[][] newWeights = new float[hiddenLayerCount + 1][];
            newWeights[0] = new float[(inputNodeCount + 1) * hiddenNodeCount];
            for (int i = 0; i < hiddenLayerCount - 1; i++)
            {
                newWeights[i + 1] = new float[(hiddenNodeCount + 1) * hiddenNodeCount];
            }
            newWeights[newWeights.Length - 1] = new float[(hiddenNodeCount + 1) * outputNodeCount];
            for (int i = 0; i < netsToCombine[0].weights.Length; i++)
            {
                for (int j = 0; j < netsToCombine[0].weights[i].Length; j++)
                {
                    newWeights[i][j] = netsToCombine[random.Next(0, numberOfNets)].weights[i][j];
                }
            }
            return new NeuralNetwork(hiddenLayerCount, inputNodeCount, hiddenNodeCount, outputNodeCount, newWeights);
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
