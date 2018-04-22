using System;
using Chess;
using System.Linq;
using System.Collections.Generic;

namespace Interface
{
    public class InputLoop
    {
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
            Position position;
            int moveLoopCursor;
            if (inputStringArray[0] == "fen")
            {
                List<String> FENStringList = new List<String>();
                for(int i = 1; i <= 6; i++)
                {
                    FENStringList.Add(inputStringArray[i]);
                }
                position = FENParser.ParseFEN(FENStringList);
                moveLoopCursor = 7;
            }
            else
            {
                position = new Position();
                moveLoopCursor = 2;
            }
            
            while (moveLoopCursor < inputStringArray.Length)
            {
                // process inputStringArray[moveLoopCursor]
                moveLoopCursor++;
            }
            ConsoleGraphics.DrawPosition(position);
        }
        private static void InputIsReady()
        {
            Console.WriteLine("readyok");
        }
        private static void InputPrint()
        {
            Console.WriteLine("pretending to print...");
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
            Console.WriteLine("id name Xysticus");
            Console.WriteLine("id author Ischa Abraham, Jeroen van den Berg");
        }
        private static void InputGo(IEnumerable<String> inputStringArray)
        {
            Console.WriteLine("pretending to search...");
        }
        private static void InputUnknown(string inputString)
        {
            Console.WriteLine(String.Format("Unknown command: {0}", inputString));
        }
        #endregion

        #region Main loop
        public static void AwaitInput()
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
