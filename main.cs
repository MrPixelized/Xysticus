using System;
using Chess;

public class Program
{
    static void Main()
    {
        Position a = new Position();

        Console.WriteLine(a.FindBestMove(1, -2, 2));
        Console.ReadKey();
    }
}
