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
        static void Main()
        {
            #region Total Training class testing
            Training training = new Training(
                reproductionOrganismCount: 2,
                maxGroupSize: 14,
                selectionFactor: 0.5f,
                populationSizeBeforeTrimming: 112,
                populationSizeAfterTrimming: 14,
                hiddenLayerCount: 2,
                inputNodeCount: 790,
                hiddenNodeCount: 32,
                outputNodeCount: 1,
                mutationRate: 0.1f,
                testFrequency: 1,
                engineDepth: 0
            );
            training.Train();
            #endregion

            #region Weight initialization testing
            /*
            NeuralNetwork net = new NeuralNetwork(2, 790, 32, 1);
            */
            #endregion

            #region EvaluatePosition testing
            /*
            float[][] weightsForOne = new float[][] { new float[] { 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f }, new float[] { 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f }, new float[] { 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f } };
            NeuralNetwork net = new NeuralNetwork(2, 2, 2, 2, weightsForOne.Select(s => s.ToArray()).ToArray());
            net.EvaluatePosition(new Position());
            */
            #endregion

            #region RunRoundRobin testing
            /*
            float[][] weightsForOne = new float[][] { new float[] { 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f }, new float[] { 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f }, new float[] { 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f } };
            float[][] weightsForTwo = new float[][] { new float[] { 2.0f, 2.0f, 2.0f, 2.0f, 2.0f, 2.0f }, new float[] { 2.0f, 2.0f, 2.0f, 2.0f, 2.0f, 2.0f }, new float[] { 2.0f, 2.0f, 2.0f, 2.0f, 2.0f, 2.0f } };

            List<NeuralNetwork> population = new List<NeuralNetwork>();
            for (int i = 0; i < 3; i++)
            {
                population.Add(new NeuralNetwork(2, 2, 2, 2, weightsForOne.Select(s => s.ToArray()).ToArray()));
                population.Add(new NeuralNetwork(2, 2, 2, 2, weightsForTwo.Select(s => s.ToArray()).ToArray()));
            }
            Training training = new Training(population, 2, 2, 0.5f, 2, 1);
            List<NeuralNetwork> strongestNetworks = training.RunRoundRobin(population);
            */
            #endregion

            #region Game class testing
            /*
            float[][] weights = new float[][] { new float[] { 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f }, new float[] { 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f }, new float[] { 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f } };
            Game game = new Game
            {
                currentPosition = FENParser.ParseFEN("3r2k1/6P1/6PB/6PK/6PP/8/8/8 b - - 0 1"),
                whitePlayer = new NeuralNetwork(2, 2, 2, 2, weights.Select(s => s.ToArray()).ToArray()),
                blackPlayer = new NeuralNetwork(2, 2, 2, 2, weights.Select(s => s.ToArray()).ToArray())
            };

            game.Play();
            */
            #endregion

            #region ToString testing
            /*
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
            */
            #endregion

            #region More training class testing

            /*
            
            Random random = new Random();

            float[][] weightsForOne = new float[][]{new float[] {1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f},new float[] { 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f }, new float[] { 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f } };
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
            Training trainingTesting = new Training(population, 2, 3, 0.5f, 10, 6);
            trainingTesting.GenerateNextGeneration();
            Console.WriteLine(trainingTesting.ToString());
            Console.ReadLine();

            */

            #endregion

            #region Training class testing
            /*
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
            Training trainingTesting = new Training(population, 2, 3, 0.5f, 20, 6);
            trainingTesting._crossover(population);
            */
            #endregion

            #region NeuralNetwork class testing
            /*
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
            float[] activations = new float[] { 2.0f, -1.0f};
            float[] thisNeedsToBeSaved = funLittleNetwork.CalculateResult(activations);
            Console.Read();
            */
            #endregion

            #region The actual interface
            /*
            NeuralNetwork net = new NeuralNetwork(2, 790, 32, 1, filename: @"C:\Users\Gebruiker\Documents\School\_PWS\strongestnetwork_dec11.txt");

            Console.WriteLine("Enter \"uci\" to enter UCI mode. ");
            string[] userInput = Console.ReadLine().Split(' ');
            if (userInput[0] == "uci")
            {
                UCIProtocol.evaluationFunction = net;
                UCIProtocol.InputLoop();
            }
            */
            #endregion

            #region Move generator testing


            /*

            Position testPosition = new Position();
            Move newMove = new Move(0, 6, 0, 5, 1);
            testPosition = testPosition.MakeMove(newMove);
            foreach((Position position, Move move) in testPosition.GeneratePositions())
            {
                ConsoleGraphics.DrawPosition(position);
            }
            Console.ReadKey();
            
            */
            #endregion

            #region More move generator testing
            /*
            Position testPosition = FENParser.ParseFEN("7k/8/8/6p1/5pP1/5Pn1/6PN/7K w - - 0 1".Split(' '));
            foreach((Position position, Move move) in testPosition.GeneratePositions())
            {
                ConsoleGraphics.DrawPosition(position);
            }
            Console.ReadKey();
            */
            #endregion

            #region Best move finder optimization

            /*

            Position a = new Position();

            System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();


            for (int c = 2; c <= 2; c++)
            {
                watch.Start();
                Console.WriteLine(string.Format("Evaluation: {0}", a.FindBestMove(c, -2, 2)));
                watch.Stop();

                Console.WriteLine(string.Format("Depth: {0} | Elapsed milliseconds: {1}", c, watch.ElapsedMilliseconds));
                watch.Reset();
            }
            Console.ReadKey();
            */
            #endregion
        }
    }
}
