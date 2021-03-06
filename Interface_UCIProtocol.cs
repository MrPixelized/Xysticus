using System;
using Chess;
using static Chess.Constants;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using static Interface.Constants;

namespace Interface
{
    public class UCIProtocol
    {
        public static Func<Position, float> evaluationFunction;
        public static Position currentPosition;

        public static string MoveToUCINotation(Move move)
        {
            StringBuilder moveStringBuilder = new StringBuilder();
            moveStringBuilder.Append(INVERSED_COORDINATE_TRANSFORMATION(move.fromX));
            moveStringBuilder.Append(8 - move.fromY);
            moveStringBuilder.Append(INVERSED_COORDINATE_TRANSFORMATION(move.toX));
            moveStringBuilder.Append(8 - move.toY);
            if (move.newPiece != move.pieceToMove)
            {
                moveStringBuilder.Append(PIECE_REPRESENTATIONS(move.newPiece).ToLower());
            }
            return moveStringBuilder.ToString();
        }

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
            int firstMove;

            if (inputStringArray[0] == "fen")
            {
                List<String> FENStringList = new List<String>();
                for (int i = 1; i <= 6; i++)
                {
                    FENStringList.Add(inputStringArray[i]);
                }
                position = FENParser.ParseFEN(FENStringList.ToArray());
                firstMove = 8;
            }
            else
            {
                position = new Position();
                firstMove = 2;
            }

            Move move;
            string moveString;

            for (int i = firstMove; i < inputStringArray.Length; i++)
            {
                moveString = inputStringArray[i];
                move = new Move(COORDINATE_TRANSFORMATION(moveString[0]), 8 - (int)Char.GetNumericValue(moveString[1]), COORDINATE_TRANSFORMATION(moveString[2]), 8 - (int)Char.GetNumericValue(moveString[3]), position.board[COORDINATE_TRANSFORMATION(moveString[0]), 8 - (int)Char.GetNumericValue(moveString[1])]);
                if ((move.pieceToMove == WHITE_PAWN || move.pieceToMove == BLACK_PAWN) && move.toX == position.enPassantSquare.Item1 && move.toY == position.enPassantSquare.Item2)
                {
                    move.enPassantCapture = true;
                }
                if (moveString.Length >= 5)
                {
                    if (position.toMove == WHITE)
                    {
                        move.newPiece = INVERSED_PIECE_REPRESENTATIONS(Char.ToUpper(moveString[4]));
                    }
                    else
                    {
                        move.newPiece = INVERSED_PIECE_REPRESENTATIONS(moveString[4]);
                    }
                }
                position = position.MakeMove(move);
            }
            currentPosition = position;
        }
        private static void InputIsReady()
        {
            Console.WriteLine("readyok");
        }
        private static void InputPrint()
        {
            Position gamePosition = currentPosition;
            ConsoleGraphics.DrawPosition(gamePosition);
            Console.WriteLine(string.Format("To move: {0}\nEn passant square: {1}\nFifty move proxmity: {2}", 
                gamePosition.toMove, gamePosition.enPassantSquare, gamePosition.fiftyMoveProximity));
            Console.Write("Castling rights: ");
            foreach (bool castlingRight in gamePosition.castlingRights)
            {
                Console.Write(castlingRight + " ");
            }
            Console.WriteLine();
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
        private static void InputGo(string[] inputStringArray)
        {
            int depth = 1;

            for(int i = 0; i < inputStringArray.Length; i++)
            {
                if (inputStringArray[i] == "depth")
                {
                    depth = int.Parse(inputStringArray[i + 1]);
                }
            }
            (Move, float) moveEvaluationTuple = Engine.FindBestMove(evaluationFunction, 
                currentPosition, depth);
            String moveString = MoveToUCINotation(moveEvaluationTuple.Item1);
            Console.WriteLine("info depth " + depth + " score cp " + 
                (int)(moveEvaluationTuple.Item2 * 100 * currentPosition.toMove) + " pv " + moveString);
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
            InputUci();
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
