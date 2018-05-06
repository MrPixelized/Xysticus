using System;
using Chess;
using ChessEngine;
using System.Linq;
using System.Text;
using System.Threading;
using System.Collections.Generic;

namespace Interface
{
    public class UCIProtocol
    {
        public static Engine engine;

        #region Input functions
        private static void InputUciNewGame()
        {
            Console.WriteLine("pretending to set up new game...");
        }
        private static void InputSetOption(IEnumerable<String> inputStringArray)
        {
            Console.WriteLine("pretending to set option...");
        }
        private static void InputPosition(string[] inputStringArray)
        {
            engine.SetPosition(inputStringArray);
            InputPrint();
        }
        private static void InputIsReady()
        {
            Console.WriteLine("readyok");
        }
        private static void InputPrint()
        {
            Position gamePosition = engine.GetPosition();
            ConsoleGraphics.DrawPosition(gamePosition);
            Console.WriteLine(String.Format("To move: {0}\nCastling rights: {0}\nEn passant square: {1}\nFifty move proxmity: {2}", gamePosition.toMove, gamePosition.castlingRights, gamePosition.enPassantSquare, gamePosition.fiftyMoveProximity));
        }
        private static void InputQuit()
        {
            Environment.Exit(1);
        }
        private static void InputStop()
        {
            Console.WriteLine("pretending to stop...");
        }
        private static void InputUci()
        {
            Console.WriteLine("id name Xysticus\n" +
                "id author Ischa Abraham, Jeroen van den Berg\n" +
                "uciok");
        }
        private static void InputGo(IEnumerable<String> inputStringArray)
        {
            Move engineMove = engine.FindBestMove();
            StringBuilder moveStringBuilder = new StringBuilder();
            moveStringBuilder.Append(Constants.INVERSED_COORDINATE_TRANSFORMATION(engineMove.fromX));
            moveStringBuilder.Append(8 - engineMove.fromY);
            moveStringBuilder.Append(Constants.INVERSED_COORDINATE_TRANSFORMATION(engineMove.toX));
            moveStringBuilder.Append(8 - engineMove.toY);
            string moveString = moveStringBuilder.ToString();
            Console.WriteLine("bestmove " + moveString);
            /*
            FindBestMoveThread = new Thread(() => game.currentPosition.FindBestMove(3, -2, 2));
            FindBestMoveThread.Start();
            Console.WriteLine("pretending to search...");
            */
        }
        private static void InputUnknown(string inputString)
        {
            Console.WriteLine(String.Format("Unknown command: {0}", inputString));
        }
        #endregion

        #region Main loop
        public static void InputLoop()
        {
            string[] inputStringArray;
            while (true)
            {
                inputStringArray = Console.ReadLine().Split(' ');
                IEnumerable<String> EnumerableInputStringArray = (IEnumerable<String>)inputStringArray;
                if (inputStringArray[0] == "ucinewgame") InputUciNewGame();
                else if (inputStringArray[0] == "setoption") InputSetOption(EnumerableInputStringArray.Skip(1));
                else if (inputStringArray[0] == "position") InputPosition(EnumerableInputStringArray.Skip(1).ToArray());
                else if (inputStringArray[0] == "isready") InputIsReady();
                else if (inputStringArray[0] == "print") InputPrint();
                else if (inputStringArray[0] == "quit") InputQuit();
                else if (inputStringArray[0] == "stop") InputStop();
                else if (inputStringArray[0] == "uci") InputUci();
                else if (inputStringArray[0] == "go") InputGo(inputStringArray.Skip(1).ToArray());
                else InputUnknown(inputStringArray[0]);
            }
        }
        #endregion
    }
}
