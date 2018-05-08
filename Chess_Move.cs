/* MOVE:
 * Move stores information that is relevant to a move, such as:
 *  Where the piece is from
 *  Where the piece is going
 *  What will be at the final square
 *  If this move is an en passant capturing move
 * !Castling information*/

namespace Chess
{
    public struct Move
    {
        public int fromX;
        public int fromY;
        public int toX;
        public int toY;
        public int newPiece;
        public int pieceToMove;
        public bool enPassantCapture;

        public Move(int x1, int y1, int x2, int y2, int pieceID, int promotionID = 0, bool enPassant = false)
        {
            fromX = x1;
            fromY = y1;
            toX = x2;
            toY = y2;
            pieceToMove = pieceID;
            if (promotionID == 0) {
                newPiece = pieceID;
            }
            else 
            {
                newPiece = promotionID;
            }
            enPassantCapture = enPassant;
        }
    }
}
