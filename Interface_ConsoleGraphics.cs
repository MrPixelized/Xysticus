using System;

namespace Interface
{
    public class ConsoleGraphics
    {
        private static string ConstructTopLine()
        {
            string topLine = Constants.BORDER_GRAPHICS("top left corner");
            for (sbyte i = 0; i < 7; i++)
            {
                topLine += Constants.BORDER_GRAPHICS("horizontal") + Constants.BORDER_GRAPHICS("top split edge");
            }
            topLine += Constants.BORDER_GRAPHICS("horizontal") + Constants.BORDER_GRAPHICS("top right corner") + "\n";
            return topLine;
        }
        private static string ConstructMiddleLine()
        {
            string middleLine = Constants.BORDER_GRAPHICS("left split edge");
            for (sbyte i = 0; i < 7; i++)
            {
                middleLine += Constants.BORDER_GRAPHICS("horizontal") + Constants.BORDER_GRAPHICS("crosspoint");
            }
            middleLine += Constants.BORDER_GRAPHICS("horizontal") + Constants.BORDER_GRAPHICS("right split edge") + "\n";
            return middleLine;
        }
        private static string ConstructMiddlePart(int[,] board)
        {
            string middlePart = "";
            for (sbyte i = 0; i < 8; i++)
            {
                for (sbyte j = 0; j < 8; j++)
                {
                    middlePart += Constants.BORDER_GRAPHICS("vertical") + Constants.PIECE_REPRESENTATIONS(board[j, i]);
                }
                middlePart += Constants.BORDER_GRAPHICS("vertical") + "\n";
                if (!(i == 7))
                {
                    middlePart += ConstructMiddleLine();
                }
            }
            return middlePart;
        }
        private static string ConstructBottomLine()
        {
            string bottomLine = Constants.BORDER_GRAPHICS("bottom left corner");
            for (sbyte i = 0; i < 7; i++)
            {
                bottomLine += Constants.BORDER_GRAPHICS("horizontal") + Constants.BORDER_GRAPHICS("bottom split edge");
            }
            bottomLine += Constants.BORDER_GRAPHICS("horizontal") + Constants.BORDER_GRAPHICS("bottom right corner");
            return bottomLine;
        }
        public static string DrawPosition(int[,] board)
        {
            return ConstructTopLine() + ConstructMiddlePart(board) + ConstructBottomLine();
        }
    }
}

