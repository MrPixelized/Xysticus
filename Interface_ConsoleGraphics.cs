using System;
using Chess;
using NNLogic;
using static Interface.Constants;

namespace Interface
{
    public class ConsoleGraphics
    {
        public static void DrawPosition(int[,] board)
        {
            Console.WriteLine(ConstructTopLine() + ConstructMiddlePart(board) + ConstructBottomLine());
        }
        public static void DrawPosition(Position position)
        {
            DrawPosition(position.board);
        }
        private static string ConstructTopLine()
        {
            string topLine = BORDER_GRAPHICS("top left corner");
            for (sbyte i = 0; i < 7; i++)
            {
                topLine += BORDER_GRAPHICS("horizontal") + BORDER_GRAPHICS("top split edge");
            }
            topLine += BORDER_GRAPHICS("horizontal") + BORDER_GRAPHICS("top right corner") + "\n";
            return topLine;
        }
        private static string ConstructMiddleLine()
        {
            string middleLine = BORDER_GRAPHICS("left split edge");
            for (sbyte i = 0; i < 7; i++)
            {
                middleLine += BORDER_GRAPHICS("horizontal") + BORDER_GRAPHICS("crosspoint");
            }
            middleLine += BORDER_GRAPHICS("horizontal") + BORDER_GRAPHICS("right split edge") + "\n";
            return middleLine;
        }
        private static string ConstructMiddlePart(int[,] board)
        {
            string middlePart = "";
            for (sbyte i = 0; i < 8; i++)
            {
                for (sbyte j = 0; j < 8; j++)
                {
                    middlePart += BORDER_GRAPHICS("vertical") + PIECE_REPRESENTATIONS(board[j, i]);
                }
                middlePart += BORDER_GRAPHICS("vertical") + "\n";
                if (!(i == 7))
                {
                    middlePart += ConstructMiddleLine();
                }
            }
            return middlePart;
        }
        private static string ConstructBottomLine()
        {
            string bottomLine = BORDER_GRAPHICS("bottom left corner");
            for (sbyte i = 0; i < 7; i++)
            {
                bottomLine += BORDER_GRAPHICS("horizontal") + BORDER_GRAPHICS("bottom split edge");
            }
            bottomLine += BORDER_GRAPHICS("horizontal") + BORDER_GRAPHICS("bottom right corner");
            return bottomLine;
        }
        public static void WriteResult(NeuralNetwork white, NeuralNetwork black, float result)
        {
            Console.Write(string.Format("\n{0,-12} {1,3} - {2,-3} {3,12}",
                string.Format("Net {0}", white.ID),
                result, 1 - result,
                string.Format("Net {0}", black.ID)
            ));
        }
    }
}

