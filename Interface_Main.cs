using ChessEngine;

namespace Interface
{
    class MainIO
    {
        static void Main()
        {
            Engine engine = new Engine();
            UCIProtocol.engine = engine;
            UCIProtocol.InputLoop();


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
        }
    }
}
