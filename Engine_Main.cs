using System;
using Chess;
using Interface;

public class Engine
{
    static void Main()
    {
        int[,] position = new int[8, 8] {
            {-4,0,0,0,-1,0,1,4},
            {-2,-1,0,0,1,0,0,2},
            {-3,-1,0,0,0,0,1,3},
            {-5,-1,0,0,0,0,1,5},
            {-6,-1,0,0,0,0,1,6},
            {-3,-1,0,0,0,0,1,3},
            {-2,-1,0,0,0,0,1,2},
            {-4,-1,0,0,0,0,1,4}
        };
        Tuple<int, int> enPassantSquare = new Tuple<int, int>(1, 5);
        //Position a = new Position(board: position, toMove: -1, enPassantSquare: enPassantSquare);
        Position a = new Position();

        System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();

        watch.Start();
        watch.Stop();
        watch.Reset();
        /*
        foreach ((Position p, Move move) in a.GeneratePositions())
        {
            if (p != null)
            {
                foreach ((Position tp, Move tmove) in p.GeneratePositions())
                {
                    if (tp != null)
                    {
                        foreach ((Position ttp, Move ttmove) in tp.GeneratePositions())
                        {
                            //if (tp != null) Console.WriteLine(ConsoleGraphics.DrawPosition(tp.board));
                        }
                    }
                }
            }
        }
        */
        for (int c=6; c<=6; c++)
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
