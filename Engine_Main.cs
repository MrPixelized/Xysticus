using System;
using Chess;
using Interface.UserInterface;
using Interface.CPUInterface;

public class Engine
{
    static void Main()
    {
        Position a = new Position();

        System.Diagnostics.Stopwatch watch;

        for (int c = 1; c < 12; c++)
        {
            watch = System.Diagnostics.Stopwatch.StartNew();
            a.FindBestMove(c, -2, 2);
            watch.Stop();
            Console.WriteLine(String.Format("Depth: {0} | Elapsed milliseconds: {1}", c, watch.ElapsedMilliseconds));
        }

        Console.ReadKey();
    }
}
