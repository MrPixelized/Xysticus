using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Chess;
using static Chess.Constants;
using Interface;
using NNLogic;

namespace Interface
{
    class MainIO
    {
        static void Main(string[] args)
        {
            for (float f = 0.01f; f < 0.1f; f += 0.01f)
            {
                Console.WriteLine(string.Format("Mutation rate: {0}, change rate: {1}", f,
                    MutationRateTest(f, 400)));
            }
            Console.ReadLine();
            return;
        }
        static void WriteRunResultsToFile()
        {
            int numberOfGames = 20;

            string[] netHistory = System.IO.File.ReadAllLines(
                @"C:\Users\Gebruiker\Documents\School\_PWS\Code\C#\bin\Release\nethistory.txt")
                .Skip(1).ToArray();
            List<NeuralNetwork> nets = new List<NeuralNetwork>();
            string netString;
            foreach (string s in netHistory)
            {
                netString = s.Split(':')[1].Substring(1);
                nets.Add(new NeuralNetwork(2, 790, 32, 1, weightsFileString: netString));
            }

            NeuralNetwork randomNet;
            float[] gameResults;
            foreach (NeuralNetwork net in nets)
            {
                gameResults = new float[numberOfGames];
                Parallel.For(0, numberOfGames, new ParallelOptions { MaxDegreeOfParallelism = 4 }, i =>
                {
                    randomNet = new NeuralNetwork(2, 790, 32, 1);
                    Game game = new Game
                    {
                        whitePlayer = i % 2 == 0 ? net : randomNet,
                        blackPlayer = i % 2 == 0 ? randomNet : net
                    };
                    game.Play();
                    gameResults[i] = i % 2 == 0 ? (float)game.result : (float)(1.0f - game.result);
                });
                Console.WriteLine(String.Format("Net gen {0}: {1}", net.ID, gameResults.Sum() / numberOfGames));
                System.IO.File.AppendAllText(@"runresults.txt", 
                    String.Format("Net gen {0}: {1}" + "\n", net.ID, gameResults.Sum() / numberOfGames));
            }

            Console.WriteLine("We're done.");
            while (true) { Console.ReadLine(); }
        }
        static void RunTraining()
        {
            Training training = new Training(
                reproductionOrganismCount: 2,
                maxGroupSize: 12,
                selectionFactor: 0.5f,
                populationSizeBeforeTrimming: 96,
                populationSizeAfterTrimming: 12,
                hiddenLayerCount: 2,
                inputNodeCount: 790,
                hiddenNodeCount: 32,
                outputNodeCount: 1,
                mutationRate: 0.01f,
                testFrequency: 1,
                engineDepth: 0,
                maxParallelGames: 5,
                numberOfTestGames: 20
            );
            training.Train();
        }
        static void RunInterface()
        {
            #region The actual interface

            NeuralNetwork net;

            if (System.IO.File.Exists("weights.txt"))
            {
                net = new NeuralNetwork(4, 790, 64, 1, filename: @"weights.txt");
            }
            else
            {
                net = new NeuralNetwork(4, 790, 64, 1, filename:
                    @"C:\Users\Gebruiker\Documents\School\_PWS\Code\C#\bin\Release\bestnetworkforfirst4x64run.txt");
            }

            net = new NeuralNetwork(2, 790, 32, 1, filename:
                @"C:\Users\Gebruiker\Documents\School\_PWS\Code\C#\bin\Release\bestnet20180108.txt");
            Console.WriteLine("Enter \"uci\" to enter UCI mode. ");
            string[] userInput = Console.ReadLine().Split(' ');
            if (userInput[0] == "uci")
            {
                UCIProtocol.evaluationFunction = net.EvaluatePosition;
                UCIProtocol.InputLoop();
            }

            #endregion
        }
        static float MutationRateTest(float mutationRate, int populationSize)
        {
            // Returns how frequently the average NN changes its preferred move in the starting position for a certain mutation rate.

            Random random = new Random();
            int hiddenLayerCount = 2;
            int inputNodeCount = 790;
            int hiddenNodeCount = 32;
            int outputNodeCount = 1;
            Move[] bestMoveArray = new Move[populationSize];

            List<NeuralNetwork> population = new List<NeuralNetwork>();
            for (int i = population.Count; i < populationSize; i++)
            {
                population.Add(new NeuralNetwork(hiddenLayerCount, inputNodeCount, hiddenNodeCount, outputNodeCount));
            }
            Position position = new Position();
            Move bestMove;
            for (int i = 0; i < population.Count; i++)
            {
                bestMoveArray[i] = Engine.FindBestMove(population[i].EvaluatePosition, position, 0).Item1;
            }

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

            int differenceCount = 0;
            for (int i = 0; i < population.Count; i++)
            {
                bestMove = Engine.FindBestMove(population[i].EvaluatePosition, position, 0).Item1;
                if (!(bestMove.fromX == bestMoveArray[i].fromX && bestMove.toX == bestMoveArray[i].toX &&
                      bestMove.fromY == bestMoveArray[i].fromY && bestMove.toY == bestMoveArray[i].toY))
                {
                    differenceCount += 1;
                }
            }
            Console.WriteLine(differenceCount.ToString(), mutationRate);
            float differenceRate = ((float)differenceCount / (float)populationSize);
            return differenceRate;
        }
        static void RandomNetworkOpeningTest()
        {
            NeuralNetwork net;

            for (int i = 0; i < 10; i++)
            {
                net = new NeuralNetwork(2, 790, 32, 1);
                net.EvaluatePosition(new Position());
                Move bestMove = Engine.FindBestMove(net.EvaluatePosition, new Position(), 1).Item1;
                Console.WriteLine(UCIProtocol.MoveToUCINotation(bestMove));
            }
            while (true) { Console.ReadLine(); }
        }
        static void StrongestNetworkTest(int numberOfGames, NeuralNetwork net = null, string filename = null)
        {
            //NeuralNetwork previousStrongest = new NeuralNetwork(2, 790, 32, 1, filename: @"C:\Users\Gebruiker\Documents\School\_PWS\strongestnetwork_dec11.txt");
            if (net == null)
            {
                net = new NeuralNetwork(2, 790, 32, 1, filename: filename);
            }
            float[] gameResults = new float[numberOfGames];
            Parallel.For(0, numberOfGames, new ParallelOptions { MaxDegreeOfParallelism = 4 }, i =>
            {
                NeuralNetwork randomNet = new NeuralNetwork(2, 790, 32, 1);
                Game game = new Game
                {
                    whitePlayer = i % 2 == 0 ? net : randomNet,
                    blackPlayer = i % 2 == 0 ? randomNet : net
                };
                game.Play();
                gameResults[i] = i % 2 == 0 ? (float)game.result : (float)(1.0f - game.result);
            });
            Console.WriteLine("Best net got " + gameResults.Sum() / numberOfGames + " average score.");

            while (true) { Console.ReadLine(); }
        }
        static void ParallelNNComputationSpeedTest()
        {
            NeuralNetwork net = new NeuralNetwork(2, 790, 32, 1);
            Stopwatch sw = new Stopwatch();
            int totalTime = 0;
            sw.Start();
            for (int i = 0; i < 1000; i++)
            {
                net.EvaluatePosition(new Position());
                sw.Stop();
                totalTime += (int)sw.ElapsedMilliseconds;
            }
            Console.WriteLine(totalTime);
            Console.Read();
        }
        static void WeightInitializationTest()
        {
            NeuralNetwork net = new NeuralNetwork(2, 790, 32, 1);
        }
        static void BestMoveFinderTest()
        {
            Position position = new Position();
            NeuralNetwork net = new NeuralNetwork(2, 790, 32, 1);

            Stopwatch watch = new Stopwatch();

            for (int c = 2; c <= 2; c++)
            {
                watch.Start();
                Console.WriteLine(string.Format("Evaluation: {0}", Engine.FindBestMove(net.EvaluatePosition, position, c)));
                watch.Stop();

                Console.WriteLine(string.Format("Depth: {0} | Elapsed milliseconds: {1}", c, watch.ElapsedMilliseconds));
                watch.Reset();
            }
            Console.ReadKey();
        }
        static void MoveGeneratorTestOne()
        {
            Position testPosition = new Position();
            Move newMove = new Move(0, 6, 0, 5, 1);
            testPosition = testPosition.MakeMove(newMove);
            foreach ((Position position, Move move) in testPosition.GeneratePositions())
            {
                ConsoleGraphics.DrawPosition(position);
            }
            Console.ReadKey();
        }
        static void MoveGeneratorTestTwo()
        {
            Position testPosition = FENParser.ParseFEN("7k/8/8/6p1/5pP1/5Pn1/6PN/7K w - - 0 1".Split(' '));
            foreach ((Position position, Move move) in testPosition.GeneratePositions())
            {
                ConsoleGraphics.DrawPosition(position);
            }
            Console.ReadKey();
        }
        static void NeuralNetworkClassTest()
        {
            float[][] weights = new float[][]
            {
                // Hidden layers
                // The first three values are for the first node (2 neurons in previous layer and the bias), the last three for the second node.
                new float[] {2.0f, 2.0f, 2.0f, 1.0f, 1.0f, 1.0f},
                new float[] {2.0f, 2.0f, 2.0f, 1.0f, 1.0f, 1.0f},
                // Output layer
                new float[] {2.0f, 2.0f, 2.0f, 1.0f, 1.0f, 1.0f}
            };
            NeuralNetwork funLittleNetwork = new NeuralNetwork(2, 2, 2, 2, weights);
            float[] activations = new float[] { 2.0f, -1.0f };
            float[] thisNeedsToBeSaved = funLittleNetwork.CalculateResult(activations);
            Console.Read();
        }
        static void TrainingClassTestOne()
        {
            Random random = new Random();

            float[][] weightsForOne = new float[][]
            {
                // Hidden layers
                // The first three values are for the first node (2 neurons in previous layer and the bias), the last three for the second node.
                new float[] {1.0f, 2.0f, 3.0f, 4.0f, 5.0f, 6.0f},
                new float[] {1.0f, 2.0f, 3.0f, 4.0f, 5.0f, 6.0f},
                // Output layer
                new float[] {1.0f, 2.0f, 3.0f, 4.0f, 5.0f, 6.0f}
            };

            float[][] weightsForTwo = new float[][]
            {
                // Hidden layers
                // The first three values are for the first node (2 neurons in previous layer and the bias), the last three for the second node.
                new float[] {2.0f, 2.0f, 2.0f, 1.0f, 1.0f, 1.0f},
                new float[] {2.0f, 2.0f, 2.0f, 1.0f, 1.0f, 1.0f},
                // Output layer
                new float[] {2.0f, 2.0f, 2.0f, 1.0f, 1.0f, 1.0f}
            };

            List<NeuralNetwork> population = new List<NeuralNetwork>()
            {
                new NeuralNetwork(2, 2, 2, 2, weightsForOne),
                new NeuralNetwork(2, 2, 2, 2, weightsForTwo),
            };
            foreach (NeuralNetwork net in population)
            {
                net.fitness = (float)random.NextDouble();
            }
            Training trainingTesting = new Training(2, 8, 0.5f, 32, 8, 2, 790, 32, 1, 0.01f, 1, 0, 4, 20, population);
            trainingTesting._crossover(population);
        }
        static void TrainingClassTestTwo()
        {
            Random random = new Random();

            float[][] weightsForOne = new float[][] { new float[] { 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f }, new float[] { 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f }, new float[] { 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f } };
            float[][] weightsForTwo = new float[][] { new float[] { 2.0f, 2.0f, 2.0f, 2.0f, 2.0f, 2.0f }, new float[] { 2.0f, 2.0f, 2.0f, 2.0f, 2.0f, 2.0f }, new float[] { 2.0f, 2.0f, 2.0f, 2.0f, 2.0f, 2.0f } };

            List<NeuralNetwork> population = new List<NeuralNetwork>();
            for (int i = 0; i < 3; i++)
            {
                population.Add(new NeuralNetwork(2, 2, 2, 2, weightsForOne.Select(s => s.ToArray()).ToArray()));
                population.Add(new NeuralNetwork(2, 2, 2, 2, weightsForTwo.Select(s => s.ToArray()).ToArray()));
            }
            foreach (NeuralNetwork net in population)
            {
                net.fitness = (float)random.NextDouble();
            }
            Training trainingTesting = new Training(2, 8, 0.5f, 32, 8, 2, 790, 32, 1, 0.01f, 1, 0, 4, 20, population);
            trainingTesting.GenerateNextGeneration();
            Console.WriteLine(trainingTesting.ToString());
            Console.ReadLine();
        }
        static void ToStringTest()
        {
            float[][] weights = new float[][]
            {
                // Hidden layers
                // The first three values are for the first node (2 neurons in previous layer and the bias), the last three for the second node.
                new float[] {2.0f, 2.0f, 2.0f, 1.0f, 1.0f, 1.0f},
                new float[] {2.0f, 2.0f, 2.0f, 1.0f, 1.0f, 1.0f},
                // Output layer
                new float[] {2.0f, 2.0f, 2.0f, 1.0f, 1.0f, 1.0f}
            };
            NeuralNetwork net = new NeuralNetwork(2, 2, 2, 2, weights);
            Console.WriteLine(net.ToString());
            Console.ReadLine();
        }
        static void GameClassTest()
        {
            float[][] weights = new float[][] { new float[] { 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f }, new float[] { 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f }, new float[] { 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f } };
            Game game = new Game
            {
                currentPosition = FENParser.ParseFEN("3r2k1/6P1/6PB/6PK/6PP/8/8/8 b - - 0 1"),
                whitePlayer = new NeuralNetwork(2, 2, 2, 2, weights.Select(s => s.ToArray()).ToArray()),
                blackPlayer = new NeuralNetwork(2, 2, 2, 2, weights.Select(s => s.ToArray()).ToArray())
            };

            game.Play();
        }
        static void RunRoundRobinTest()
        {
            float[][] weightsForOne = new float[][] { new float[] { 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f }, new float[] { 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f }, new float[] { 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f } };
            float[][] weightsForTwo = new float[][] { new float[] { 2.0f, 2.0f, 2.0f, 2.0f, 2.0f, 2.0f }, new float[] { 2.0f, 2.0f, 2.0f, 2.0f, 2.0f, 2.0f }, new float[] { 2.0f, 2.0f, 2.0f, 2.0f, 2.0f, 2.0f } };

            List<NeuralNetwork> population = new List<NeuralNetwork>();
            for (int i = 0; i < 3; i++)
            {
                population.Add(new NeuralNetwork(2, 2, 2, 2, weightsForOne.Select(s => s.ToArray()).ToArray()));
                population.Add(new NeuralNetwork(2, 2, 2, 2, weightsForTwo.Select(s => s.ToArray()).ToArray()));
            }
            Training training = new Training(2, 8, 0.5f, 32, 8, 2, 790, 32, 1, 0.01f, 1, 0, 4, 20, population);
            List<NeuralNetwork> strongestNetworks = training.RunRoundRobin(population);
        }
        static void EvaluatePositionTest()
        {
            float[][] weightsForOne = new float[][] { new float[] { 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f }, new float[] { 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f }, new float[] { 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f } };
            NeuralNetwork net = new NeuralNetwork(2, 2, 2, 2, weightsForOne.Select(s => s.ToArray()).ToArray());
            net.EvaluatePosition(new Position());
        }
    }
}
