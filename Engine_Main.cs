using System;
using Chess;
using Interface;

public class Engine
{
    static void Main()
    {
        Position a = new Position();

        System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();


        for (int c=8; c<=8; c++)
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
