namespace BoardPrinting
{
    class BoardPrinter
    {
        private string PIECE_REPRESENTATIONS(sbyte pieceNumber)
        {
            switch (pieceNumber)
            {
                case 0: return " ";
                case 1: return "P";
                case 2: return "N";
                case 3: return "B";
                case 4: return "R";
                case 5: return "Q";
                case 6: return "K";
                case 7: return " ";
                case -1: return "p";
                case -2: return "n";
                case -3: return "b";
                case -4: return "r";
                case -5: return "q";
                case -6: return "k";
                case -7: return " ";
                default: return " ";
            }
        }
        private string BORDER_GRAPHICS(string graphicsSpecification)
        {
            switch (graphicsSpecification)
            {
                case "top left corner": return "┌";
                case "top right corner": return "┐";
                case "bottom left corner": return "└";
                case "bottom right corner": return "┘";
                case "crosspoint": return "┼";
                case "top split edge": return "┬";
                case "bottom split edge": return "┴";
                case "left split edge": return "├";
                case "right split edge": return "┤";
                case "horizontal": return "─";
                case "vertical": return "│";
                default: return " ";
            }
        }
        private string ConstructTopLine()
        {
            string topLine = BORDER_GRAPHICS("top left corner");
            for (sbyte i = 0; i < 7; i++)
            {
                topLine += BORDER_GRAPHICS("horizontal") + BORDER_GRAPHICS("top split edge");
            }
            topLine += BORDER_GRAPHICS("horizontal") + BORDER_GRAPHICS("top right corner") + "\n";
            return topLine;
        }
        private string ConstructMiddleLine()
        {
            string middleLine = BORDER_GRAPHICS("left split edge");
            for (sbyte i = 0; i < 7; i++)
            {
                middleLine += BORDER_GRAPHICS("horizontal") + BORDER_GRAPHICS("crosspoint");
            }
            middleLine += BORDER_GRAPHICS("horizontal") + BORDER_GRAPHICS("right split edge") + "\n";
            return middleLine;
        }
        private string ConstructMiddlePart(sbyte[,] board)
        {
            string middlePart = "";
            for (sbyte i = 0; i < 8; i++)
            {
                for (sbyte j = 0; j < 8; j++)
                {
                    middlePart += BORDER_GRAPHICS("vertical") +  PIECE_REPRESENTATIONS(board[j, i]);
                }
                middlePart += BORDER_GRAPHICS("vertical") + "\n";
                if (!(i == 7)) {
                    middlePart += ConstructMiddleLine();
                }
            }
            return middlePart;
        }
        private string ConstructBottomLine()
        {
            string bottomLine = BORDER_GRAPHICS("bottom left corner");
            for (sbyte i = 0; i < 7; i++)
            {
                bottomLine += BORDER_GRAPHICS("horizontal") + BORDER_GRAPHICS("bottom split edge");
            }
            bottomLine += BORDER_GRAPHICS("horizontal") + BORDER_GRAPHICS("bottom right corner");
            return bottomLine;
        }
        public string DrawPosition(sbyte[,] board)
        {
            return ConstructTopLine() + ConstructMiddlePart(board) + ConstructBottomLine();
        }
    }
}
