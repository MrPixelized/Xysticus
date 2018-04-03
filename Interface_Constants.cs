namespace Interface
{
    internal static class Constants
    {
        public static string PIECE_REPRESENTATIONS(int pieceNumber)
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

        public static string BORDER_GRAPHICS(string graphicsSpecification)
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
    }
}

