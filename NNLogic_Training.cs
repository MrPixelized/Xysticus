using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        readonly int maxParallelGames;
        readonly int numberOfTestGames;
        public List<NeuralNetwork> population;
        int generation;
        NeuralNetwork previousBest;
        Random random;

        public Training(int reproductionOrganismCount, int maxGroupSize, float selectionFactor,
            int populationSizeBeforeTrimming, int populationSizeAfterTrimming, int hiddenLayerCount,
            int inputNodeCount, int hiddenNodeCount, int outputNodeCount, float mutationRate, int testFrequency,
            int engineDepth, int maxParallelGames, int numberOfTestGames, List<NeuralNetwork> population = null,
            string populationFile = null)
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
            this.maxParallelGames = maxParallelGames;
            this.numberOfTestGames = numberOfTestGames;
            if (population != null)
            {
                this.population = population;
            }
            else if (populationFile != null)
            {
                this.population = FileToPopulation(populationFile);
            }
            else
            {
                this.population = InitializePopulation();
            }
            random = new Random();
        }

        public List<NeuralNetwork> FileToPopulation(string populationFile)
        {
            List<NeuralNetwork> initialPopulation = new List<NeuralNetwork>();
            string allLines = System.IO.File.ReadAllLines(populationFile)[0];
            string[] lineArrays = allLines.Split(new string[] { "}}" }, StringSplitOptions.None);
            lineArrays = lineArrays.Take(lineArrays.Length - 1).ToArray();
            for (int i = 0; i< lineArrays.Length; i++)
            {
                lineArrays[i] = lineArrays[i] + "}}";
            }
            for (int i = 0; i < lineArrays.Length; i++)
            {
                initialPopulation.Add(new NeuralNetwork(hiddenLayerCount, inputNodeCount, hiddenNodeCount,
                    outputNodeCount, weightsFileString: lineArrays[i])
                {
                    fitness = 1.0f
                });
            }
            return initialPopulation;
        }

        public List<NeuralNetwork> InitializePopulation()
        {
            List<NeuralNetwork> initialPopulation = new List<NeuralNetwork>();
            for (int i = initialPopulation.Count; i < populationSizeBeforeTrimming; i++)
            {
                initialPopulation.Add(new NeuralNetwork(hiddenLayerCount, inputNodeCount, hiddenNodeCount, outputNodeCount));
            }
            return initialPopulation;
        }

        public void Train()
        {
            if (population.Count < populationSizeBeforeTrimming)
            {
                GenerateNextGeneration();
            }
            previousBest = population[0];
            ClearFiles();
            while (true)
            {
                generation += 1;
                SelectionTournament();
                SetFitnesses();
                SortNets();
                ResetNetValues();
                SaveAllToFile();
                if (generation % testFrequency == 0)
                {
                    SaveBestToFile();
                    CompareBestToRandom();
                }
                GenerateNextGeneration();
                MutatePopulation();
                Console.WriteLine("Generation {0} has passed.", generation);
            }
        }

        public void ClearFiles()
        {
            Console.WriteLine("Please only continue if you want to delete the files nethistory.txt and movehistory.pgn");
            Console.ReadLine();
            System.IO.File.WriteAllText("nethistory.txt", "");
            System.IO.File.WriteAllText("movehistory.pgn", "");
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

        }

        public void ResetNetValues()
        {
            foreach (NeuralNetwork net in population)
            {
                net.score = 0;
                net.totalScore = 0;
                net.games = 0;
                net.totalGames = 0;
            }
        }

        public void SaveBestToFile()
        {
            System.IO.File.AppendAllText("nethistory.txt", "\nGeneration " + generation + ": ");
            System.IO.File.AppendAllText("nethistory.txt", population[0].ToString());
        }

        public void SaveAllToFile()
        {
            System.IO.File.WriteAllText("lastrunnetworks.txt", "");
            for (int i = 0; i < population.Count; i++)
            {
                System.IO.File.AppendAllText("lastrunnetworks.txt", population[i].ToString());
            }
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

        public void CompareBestToRandom()
        {
            NeuralNetwork bestNet = population[0];
            float[] gameResults = new float[numberOfTestGames];
            NeuralNetwork randomNet;
            Parallel.For(0, numberOfTestGames, new ParallelOptions { MaxDegreeOfParallelism = maxParallelGames }, i =>
            {
                randomNet = new NeuralNetwork(2, 790, 32, 1);
                Game game = new Game
                {
                    whitePlayer = i % 2 == 0 ? bestNet : randomNet,
                    blackPlayer = i % 2 == 0 ? randomNet : bestNet
                };
                game.Play();
                gameResults[i] = i % 2 == 0 ? (float)game.result : (float)(1.0f - game.result);
            });
            Console.WriteLine(String.Format("Net gen {0}: {1}", generation, gameResults.Sum() / numberOfTestGames));
            System.IO.File.AppendAllText(@"runresults.txt",
                String.Format("Net gen {0}: {1}" + "\n", generation, gameResults.Sum() / numberOfTestGames));
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
                        if (random.NextDouble() < mutationRate)
                        {
                            weightArray[i] = (float)random.NextDouble();
                        }
                    }
                }
            }
        }

        public List<NeuralNetwork> RunRoundRobin(List<NeuralNetwork> nets)
        {
            // Return the strongest ceil(selectionFactor * nets.Count) nets
            Console.Write("\nNew round robin: ");

            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            int numberOfNets = nets.Count;
            int numberOfGames = numberOfNets * (numberOfNets - 1) / 2;
            float[,] gameResults = new float[numberOfNets,numberOfNets];
            sw.Start();
            Parallel.ForEach(
                GeneratePairings(nets),
                new ParallelOptions { MaxDegreeOfParallelism = maxParallelGames },
                game =>
                {
                    game.Play();
                    gameResults[nets.IndexOf(game.whitePlayer),nets.IndexOf(game.blackPlayer)] = (float)game.result;
                    gameResults[nets.IndexOf(game.blackPlayer),nets.IndexOf(game.whitePlayer)] = 1.0f - (float)game.result;
                    //ConsoleGraphics.WriteResult(game.whitePlayer, game.blackPlayer, (float)game.result);
                    game.whitePlayer.games++; game.blackPlayer.games++;
                }
            );
            for (int i = 0; i < numberOfNets; i++)
            {
                for (int j = 0; j < numberOfNets; j++)
                {
                    nets[i].score += gameResults[i, j];
                }
            }
            sw.Stop();
            Console.WriteLine("\nAverage game time: " + sw.ElapsedMilliseconds / ((numberOfNets * (numberOfNets - 1)) / 2) + " ms.");
            
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
                    Game game = new Game
                    {
                        whitePlayer = i % 2 == j % 2 ? nets[i] : nets[j],
                        blackPlayer = i % 2 == j % 2 ? nets[j] : nets[i],
                        engineDepth = engineDepth
                    };
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

        public NeuralNetwork _crossover(List<NeuralNetwork> netsToCombine)
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
