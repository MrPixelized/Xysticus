from random import random
from copy import deepcopy, copy
import numpy as np

"""SQUARE-PIECE VALUES:
    0: empty
    1: pawn
    2: knight
    3: bishop
    4: rook
    5: queen
    6: king
    7: en passant square
    negative values represent black pieces"""
    
"""COLOR REPRESENTATIONS:
    -1: black
     1: white"""

"""CASTLING RIGHTS REPRESENTATION:
    Castling rights are represented as a tuple of four binary values, ordered as follows:
      (Whites castling rights, Blacks castling rights)
    True values indicate the presence of the right, false values indicate the opposite"""

STANDARD_POSITION = [
  [-4,-1,0,0,0,0,1,4],
  [-2,-1,0,0,0,0,1,2],
  [-3,-1,0,0,0,0,1,3],
  [-5,-1,0,0,0,0,1,5],
  [-6,-1,0,0,0,0,1,6],
  [-3,-1,0,0,0,0,1,3],
  [-2,-1,0,0,0,0,1,2],
  [-4,-1,0,0,0,0,1,4]
]

PIECE_REPRESENTATIONS = {
   0:" ",
   1: "P",
   2: "N",
   3: "B",
   4: "R",
   5: "Q",
   6: "K",
   7: "E",
  -1: "p",
  -2: "n",
  -3: "b",
  -4: "r",
  -5: "q",
  -6: "k",
  -7: "e"
}

MOVE_DIRECTIONS = {
  -6: [(1,1),(-1,1),(-1,-1),(1,-1),(0,1),(0,-1),(1,0),(-1,0),(2,0),(-2,0)],
  -5: [(1,1),(-1,1),(-1,-1),(1,-1),(0,1),(0,-1),(1,0),(-1,0)],
  -4: [(0,1),(0,-1),(1,0),(-1,0)],
  -3: [(1,1),(-1,1),(-1,-1),(1,-1)],
  -2: [(1,2),(-1,2),(1,-2),(-1,-2)],
  -1: [(0,-1),(1,-1),(-1,-1),(0,-2)], 
   1: [(0,1),(1,1),(-1,1),(0,2)],
   2: [(1,2),(-1,2),(1,-2),(-1,-2)],
   3: [(1,1),(-1,1),(-1,-1),(1,-1)],
   4: [(0,1),(0,-1),(1,0),(-1,0)],
   5: [(1,1),(-1,1),(-1,-1),(1,-1),(0,1),(0,-1),(1,0),(-1,0)],
   6: [(1,1),(-1,1),(-1,-1),(1,-1),(0,1),(0,-1),(1,0),(-1,0),(2,0),(-2,0)],
}

class Position():
  def __init__(self, position=STANDARD_POSITION, toMove=1, fiftyMoveProximity=0, castlingRights=[True, True]):
    self.squareArray = position
    self.toMove = toMove
    self.fiftyMoveProximity = fiftyMoveProximity
    
  def findBestMove(self, depth):
    # Generate all possible positions
    for position, move in self.generatePositions():
    # If depth > 1, call findBestMove on every yielded position, decrease depth by one
      if depth > 1:
        position.findBestMove(depth - 1)
    # Else, evaluate all of the yielded positions
      else:
        evaluation = evaluationNetwork(position)
        if evaluation > bestEvaluation:
          bestEvaluation, bestMove = evaluation, move
    # From these positions, return the highest value if it's the engine to play
    # else return the the lowest value
    # In both cases return the move made
    
  def generatePositions(self):
    # Iterate through all of the squares on the board
    for squareContent, x, y in self._iteratePosition():
      # Check if the resulting square contains a piece
      if _isPiece(squareContent) and squareContent * self.toMove > 0:
        # Loop through each element of the corresponding move directions array
        for move in MOVE_DIRECTIONS[abs(squareContent)]:
          while True:
            # Test if a move is possible, disregarding checks
            if not self._legalMove(x, y, move[0], move[1]):
              break
            # If the move is possible, yield the position that would arise
            else:
              yield self.makeMove(x, y, move[0], move[1]), (x, y, move[0], move[1])
            # If the piece is a king, a knight or a pawn (all pieces that move only once in every direction)
            # break from the loop, else, keep making the same move until it is no longer legal
            if _isKing(squareContent) or _isKnight(squareContent) or _isPawn(squareContent):
              break
    
  def makeMove(self, x, y, moveX, moveY):
    # Important variables are created
    pieceToMove = self.squareArray[x][y]
    squareToMoveTo = self.squareArray[moveX][moveY]
    # Copies of all relevant variables are made
    newSquareArray = deepcopy(self.squareArray)
    newCastlingRights = copy(self.castlingRights)
    newColorToMove = self.toMove * -1
    newFiftyMoveProximity = self.fiftyMoveProximity
    # The en passant square is removed (if it exists)
    newSquareArray = _removeEnPassant(newSquareArray)
    # If the square to move to is not empty, the fifty move proximity is reset
    if squareToMoveTo != 0: newFiftyMoveProximity = 0
    # Then, exceptional moves are handled
    # If the piece to move is a king, a castling request is considered
    if _isKing(pieceToMove):
      # If the king wants to castle, the rook is moved in advance
      if x - moveX == 2:
        if y == 0:
          newSquareArray[x-1][y] = -4
          newSquareArray[0][y] = 0
        else:
          newSquareArray[x-1][y] = 4
          newSquareArray[0][y] = 0
      elif x - moveX == -2:
        if y == 0:
          newSquareArray[x+1][y] = -4
          newSquareArray[7][y] = 0
        else:
          newSquareArray[x+1][y] = 4
          newSquareArray[7][y] = 0
      # Also, castling rights are set according to the color to move
      if pieceToMove < 0: newCastlingRights[1] = False
      if pieceToMove > 0: newCastlingRights[0] = False
    # If the piece to move is a pawn, a double move request is considered, also, the fifty move proximity is reset
    if _isPawn(self.squareArray[x][y]):
      newFiftyMoveProximity = 0
      # If a pawn wants to move forward by two squares, the en passant square is put in
      if y - moveY == -2:
        newSquareArray[x][y+1] = 7
      elif y - moveY == 2:
        newSquareArray[x][y-1] = -7

    # Finally, the piece is moved to the desired spot
    newSquareArray[x][y], newSquareArray[moveX][moveY] = 0, pieceToMove
    
    return Position(newSquareArray, newColorToMove, newFiftyMoveProximity, newCastlingRights)
    
  def __eq__(self, other):
    return self.squareArray == other.squareArray and self.toMove == other.toMove
    
  def __str__(self):
    toString = ""
    # If we add the top line, the middle part, and the bottom line, we get the board.
    return self._printTopLine() + "\n" + self._printMiddlePart() + "\n" + self._printBottomLine()
  def _printTopLine(self):
    # Printing the top line of the board, which is something like this: ┌─┬─┬─┬─┬─┬─┬─┬─┐
    return borderGraphics["top left corner"] + 7 * (borderGraphics["horizontal"] + borderGraphics["top split edge"]) + borderGraphics["horizontal"] + borderGraphics["top right corner"]
  def _printMiddleLine(self):
    # Printing a middle line of the board, which is something like this: ├─┼─┼─┼─┼─┼─┼─┼─┤
    return "\n" + borderGraphics["left split edge"] + 7 * (borderGraphics["horizontal"] + borderGraphics["crosspoint"]) + borderGraphics["horizontal"] + borderGraphics["right split edge"] + "\n"
  def _printMiddlePart(self):
    # The middle part is each row of pieces, all separated by middle lines.
    i = 0
    toString = ""
    for rank in self._turnedBoard():
      i += 1
      # Each piece row exists of 9 vertical bars, with the pieces in between them.
      toString += borderGraphics["vertical"]
      for square in rank:
        toString += PIECE_REPRESENTATIONS[square] + borderGraphics["vertical"]
      if not i == 8:
        # The final row does not need a middle line.
        toString += self._printMiddleLine()
    return toString

  def _printBottomLine(self):
    # Printing the bottom line of the board, which is like this: └─┴─┴─┴─┴─┴─┴─┴─┘
    return borderGraphics["bottom left corner"] + 7 * (borderGraphics["horizontal"] + borderGraphics["bottom split edge"]) + borderGraphics["horizontal"] + borderGraphics["bottom right corner"]
  def _turnedBoard(self):
    return ([[y[x] for y in self.squareArray] for x in range(8)])
    
  def _convertPositionToString(self):
    # Flatten the position to one dimension
    boardArray = flattenToOneDimension(self.squareArray)
    # Append the color to move to the array
    return
  
  def _iteratePosition(self):
    for x in self.squareArray:
      for y in x:
        yield self.squareArray[x][y], x, y
    
  def _legalMove(self, x, y, moveX, moveY):
    # Test if the move puts the piece out of bounds
    if -1 < y + moveY < 8 or -1 < x + moveX < 8:
      return False
    # Initialize a variable that stores the piece type to be moved
    pieceToMove = self.squareArray[x][y]
    # Initialize a  variable that stores the content of the square to be moved to
    squareToMoveTo = self.squareArray[moveX][moveY]
    # Initialize a variable that indicates the color of the moved piece compared to the content of the square to go to
    colorIndication = pieceToMove * squareToMoveTo
    # Test if the piece to move is a pawn
    if _isPawn(pieceToMove):
      # If the piece to move is a pawn, test if the move trying to be made is a double move and if so, test if it is legal to make this move
      if y-moveY == 2 and not y == 6:
        return False
      elif y-moveY == -2 and not y == 1:
        return False
      # If the piece to move is a pawn, test if the move trying to be made is a capture move, and if so, test if it is legal to make this move
      elif (x-moveX == 1 or x-moveX == -1) and colorIndication >= 0:
        return False

    # Test if the piece to move is a king
    if _isKing(pieceToMove):
      # If the piece to move is a king, test if the move to be made is a castling move
      if x-moveX == 2: # Short castling
        if pieceToMove < 0: # Black castling
          if not y == 0:
            return False
          if not self.castlingRights[1]:
            return False
          if not 
    # Finally, evaluate if the move doesn't land a piece on a friendly square
    return colorIndication <= 0
  
  def _convertToInputs(self):
    # Flatten the squareArray
    return flattenToOneDimension(self.squareArray)

def _removeEnPassant(squareArray):
  return [[0 if y == 7 else y for y in x] for x in newSquareArray]

def _isPiece(squareContent):
  return -7 < squareContent < 7
    
def _isPawn(squareContent):
  return abs(squareContent) == 1

def _isKing(squareContent):
  return abs(squareContent) == 6
  
def _convertIdToRepresentation(id):
  return PIECE_REPRESENTATIONS[id]
    
def flattenToOneDimension(twoDimensionalArray):
  # Take a two dimensional array and zip all of the internal lists together to form one one dimensional array
  return [y for x in twoDimensionalArray for y in x]

def rotateTwoDimensionalArray(twoDimensionalArray):
  # Take a two dimensional array and rotate it 90 degrees anticlockwise
  return [[x[y] for x in twoDimensionalArray] for y in range(len(twoDimensionalArray))]

def evaluationNetwork(position):
  inputs = position._convertToInputs()
  return random()
  
a = Position()

for p in a.generatePositions():
  print(p)

while True:
  strInput = input().split(" ")
  if strInput[0] == "uci":
    print("""id name Xysticus\nid author Ischa Abraham, Jeroen van den Berg""")
  elif strInput[0] == "go":
    moveFirst, moveLast = findMove(currentPosition)
    currentPosition = currentPosition.makeMove()
  elif strInput[0] == "print":
    print(currentPosition)
  else:
    print("Unknown command: %s" % (strInput[0]))
    
    
