using System;
using Chess;
using Interface;

public class Engine
{
    static void Main()
    {
        int[,] b = new int[8, 8] {
            {0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0},
            {0,0,1,0,0,0,0,0},
            {0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,1,0},
            {0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0}
        };
    Position a = new Position(b);

        System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();


        for (int c=3; c<=3; c++)
        {
            watch.Start();
            Console.WriteLine(string.Format("Evaluation: {0}", a.FindBestMove(c, -2, 2)));
            watch.Stop();

            Console.WriteLine(string.Format("Depth: {0} | Elapsed milliseconds: {1}", c, watch.ElapsedMilliseconds));
            watch.Reset();
        }
        Console.ReadKey();
    }
}
