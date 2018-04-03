using System;
using Chess;
using Interface.UserInterface;
using Interface.CPUInterface;

public class Engine
{
    static void Main()
    {
        Position a = new Position();

        Console.WriteLine(ConsoleGraphics.DrawPosition(a.board));
        Console.WriteLine(a.FindBestMove(1, -2, 2));
        Console.ReadKey();
    }
}
