PIECE_REPRESENTATIONS = {
  0:" ",
  1:"P",
  2:"N",
  3:"B",
  4:"R",
  5:"Q",
  6:"K",
  7:" ",
  -1:"p",
  -2:"n",
  -3:"b",
  -4:"r",
  -5:"q",
  -6:"k",
  -7:" "
}

BORDER_GRAPHICS = {
    "top left corner":"┌",
    "top right corner":"┐",
    "bottom left corner":"└",
    "bottom right corner":"┘",
    "crosspoint":"┼",
    "top split edge":"┬",
    "bottom split edge":"┴",
    "left split edge":"├",
    "right split edge":"┤",
    "horizontal":"─",
    "vertical":"│"
}

def drawPosition(squareArray):
  return _printTopLine() + "\n" + _printMiddlePart(squareArray) + "\n" + _printBottomLine() 

def _printTopLine():
  """┌─┬─┬─┬─┬─┬─┬─┬─┐"""
  return BORDER_GRAPHICS["top left corner"] + 7 * (BORDER_GRAPHICS["horizontal"] + BORDER_GRAPHICS["top split edge"]) + BORDER_GRAPHICS["horizontal"] + BORDER_GRAPHICS["top right corner"]
def _printMiddleLine():
  # Printing the middle line of the board, looking something like this: ├─┼─┼─┼─┼─┼─┼─┼─┤
  return "\n" + BORDER_GRAPHICS["left split edge"] + 7 * (BORDER_GRAPHICS["horizontal"] + BORDER_GRAPHICS["crosspoint"]) + BORDER_GRAPHICS["horizontal"] + BORDER_GRAPHICS["right split edge"] + "\n"
def _printMiddlePart(squareArray):
  i = 0
  toString = ""
  for rank in _turnedBoard(squareArray):
    i += 1
    toString += BORDER_GRAPHICS["vertical"]
    for square in rank:
      toString += PIECE_REPRESENTATIONS[square] + BORDER_GRAPHICS["vertical"]
    if not i == 8:
      toString += _printMiddleLine()
  return toString
def _printBottomLine():
  # Printing the bottom line of the board, looking something like this: ├─┼─┼─┼─┼─┼─┼─┼─┤ └─┴─┴─┴─┴─┴─┴─┴─┘
  return BORDER_GRAPHICS["bottom left corner"] + 7 * (BORDER_GRAPHICS["horizontal"] + BORDER_GRAPHICS["bottom split edge"]) + BORDER_GRAPHICS["horizontal"] + BORDER_GRAPHICS["bottom right corner"]
def _turnedBoard(squareArray):
  return ([[y[x] for y in squareArray] for x in range(8)])
