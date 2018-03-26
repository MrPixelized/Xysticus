//from defs import _turnedBoard
//

Dictionary<sbyte, char> PIECE_REPRESENTATIONS = new Dictionary<sbyte, char>{
	{0, ' '},
	{1, 'P'},
	{2, 'N'},
	{3, 'B'},
	{4, 'R'},
	{5, 'Q'},
	{6, 'K'},
	{7, ' '},
	{-1, 'p'},
	{-2, 'n'},
	{-3, 'b'},
	{-4, 'r'},
	{-5, 'q'},
	{-6, 'k'},
	{-7, ' '}
};

Dictionary<string, char> BORDER_GRAPHICS = new Dictionary<string, char>{
	{"top left corner", '┌'},
	{"top right corner", '┐'},
	{"bottom left corner", '└'},
	{"bottom right corner", '┘'},
	{"crosspoint", '┼'},
	{"top split edge", '┬'},
	{"bottom split edge", '┴'},
	{"left split edge", '├'},
	{"right split edge", '┤'},
	{"horizontal", '─'},
	{"vertical", '│'}
};

//def drawPosition(squareArray):
//  # If we add the top line, the middle part, and the bottom line, we get the board.
//  return _printTopLine() + "\n" + _printMiddlePart(squareArray) + "\n" + _printBottomLine() 

string drawPosition(int[][] board){
	return _printTopLine() + '\n' + _printMiddlePart(squareArray) + '\n' + _printBottomLine();
}

//def _printTopLine():
//  # Printing the top line of the board, which is something like this: ┌─┬─┬─┬─┬─┬─┬─┬─┐
//  return BORDER_GRAPHICS["top left corner"] + 7 * (BORDER_GRAPHICS["horizontal"] + BORDER_GRAPHICS["top split edge"]) + BORDER_GRAPHICS["horizontal"] + BORDER_GRAPHICS["top right corner"]
//def _printMiddleLine():
//  # Printing a middle line of the board, which is something like this: ├─┼─┼─┼─┼─┼─┼─┼─┤
//  return "\n" + BORDER_GRAPHICS["left split edge"] + 7 * (BORDER_GRAPHICS["horizontal"] + BORDER_GRAPHICS["crosspoint"]) + BORDER_GRAPHICS["horizontal"] + BORDER_GRAPHICS["right split edge"] + "\n"
//def _printMiddlePart(squareArray):
//  # The middle part is each row of pieces, all separated by middle lines.
//  i = 0
//  toString = ""
//  for rank in _turnedBoard(squareArray):
//    i += 1
//    # Each piece row exists of 9 vertical bars, with the pieces in between them.
//    toString += BORDER_GRAPHICS["vertical"]
//    for square in rank:
//      toString += _convertIdToRepresentation(square) + BORDER_GRAPHICS["vertical"]
//    if not i == 8:
//      # The final row does not need a middle line.
//      toString += _printMiddleLine()
//  return toString
//def _printBottomLine():
//  # Printing the bottom line of the board, which is something like this: └─┴─┴─┴─┴─┴─┴─┴─┘
//  return BORDER_GRAPHICS["bottom left corner"] + 7 * (BORDER_GRAPHICS["horizontal"] + BORDER_GRAPHICS["bottom split edge"]) + BORDER_GRAPHICS["horizontal"] + BORDER_GRAPHICS["bottom right corner"]
//def _convertIdToRepresentation(id):
//  return PIECE_REPRESENTATIONS[id]
//