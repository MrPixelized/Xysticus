using System;
using Chess;
using Interface;

public class Engine
{
    static void Main()
    {
        Position a = new Position();

        InputLoop.AwaitInput();
        Console.WriteLine(a.FindBestMove(1, -2, 2));
        Console.ReadKey();
    }
}
