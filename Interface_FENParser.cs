using System;
using System.Collections.Generic;
using Chess;

namespace Interface
{
    public class FENParser
    {
        /*
        private static int[,] ParseBoard(string FENBoard)
        {
            int[,] board = new int[8,8];
            foreach (string rank in FENBoard.Split('/'))
            {
                foreach (char character in rank)
                {
                    if (Char.IsNumber(character)) {

                    }
                }
            }
            return board;
        }
        */

        public static Position ParseFEN(List<String> FENString)
        {
            // TODO: parse piece placement
            // TODO: parse active color
            // TODO: parse castling availability
            // TODO: parse en passant target square
            // TODO: parse halfmove clock
            // TODO: parse fullmove number 
            return new Position();
        }
    }
}
