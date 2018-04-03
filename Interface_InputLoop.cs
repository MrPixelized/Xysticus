using System;
using System.Windows.Forms;

namespace Interface
{
    public class InputLoop
    {
        private static void InputUciNewGame()
        {
            Console.WriteLine("pretending to set up new game...");
        }
        private static void InputSetOption(string inputString)
        {
            Console.WriteLine("pretending to set option...");
        }
        private static void InputPosition(string inputString)
        {
            string[] inputArray = inputString.Split(new string[] { " moves " }, StringSplitOptions.None);
            Console.WriteLine("pretending to set position...");
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
            Console.WriteLine("pretending to quit...");
        }
        private static void InputStop()
        {
            Console.WriteLine("pretending to stop...");
        }
        private static void InputUci()
        {
            Console.WriteLine("pretending to give some info...");
        }
        private static void InputGo(string inputString)
        {
            Console.WriteLine("pretending to search...");
        }
        public static void AwaitInput()
        {
            string inputString;
            while (true)
            {
                inputString = Console.ReadLine();
                if (inputString == "ucinewgame") InputUciNewGame();
                else if (inputString.StartsWith("setoption ")) InputSetOption(inputString.Substring(10));
                else if (inputString.StartsWith("position ")) InputPosition(inputString.Substring(9));
                else if (inputString == "isready") InputIsReady();
                else if (inputString == "print") InputPrint();
                else if (inputString == "quit") InputQuit();
                else if (inputString == "stop") InputStop();
                else if (inputString == "uci") InputUci();
                else if (inputString.StartsWith("go ")) InputGo(inputString.Substring(3));
            }
        }
    }
}