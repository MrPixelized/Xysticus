from random import random
from copy import deepcopy, copy
import numpy as np
import graphics
import neuralnet

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
    Permanent castling rights are represented as a tuple of four binary values, ordered as follows:
      (White's short castling right, 
      White's long castling right,
      Black's short castling right,
      Black's long castling right)
    True values indicate the presence of the right, False values indicate the opposite"""

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

MOVE_DIRECTIONS = {
  -6: [(1,1),(-1,1),(-1,-1),(1,-1),(0,1),(0,-1),(1,0),(-1,0),(2,0),(-2,0)],
  -5: [(1,1),(-1,1),(-1,-1),(1,-1),(0,1),(0,-1),(1,0),(-1,0)],
  -4: [(0,1),(0,-1),(1,0),(-1,0)],
  -3: [(1,1),(-1,1),(-1,-1),(1,-1)],
  -2: [(1,2),(-1,2),(1,-2),(-1,-2)],
  -1: [(0,1),(1,1),(-1,1),(0,2)],
   1: [(0,-1),(1,-1),(-1,-1),(0,-2)],
   2: [(1,2),(-1,2),(1,-2),(-1,-2)],
   3: [(1,1),(-1,1),(-1,-1),(1,-1)],
   4: [(0,1),(0,-1),(1,0),(-1,0)],
   5: [(1,1),(-1,1),(-1,-1),(1,-1),(0,1),(0,-1),(1,0),(-1,0)],
   6: [(1,1),(-1,1),(-1,-1),(1,-1),(0,1),(0,-1),(1,0),(-1,0),(2,0),(-2,0)]
}

class Position():
  def __init__(self, position=STANDARD_POSITION, toMove=1, fiftyMoveProximity=0, castlingRights=[True, True, True, True]):
    self.squareArray = position
    self.toMove = toMove
    self.fiftyMoveProximity = fiftyMoveProximity
    self.castlingRights = castlingRights

  def findBestMove(self, depth):
    finalEvaluation = self.toMove # This is a good starting value, given that the evaluation always lands between -1 and 1
    for position, move in self.generatePositions():
      if position._kingNotInPosition():
        return False
      if depth <= 1:
        return neuralnet.evaluationNetwork(position)
      else:
        evaluation = position.findBestMove(depth - 1)
      if evaluation == False:
        print("This position will never be played")
        continue
      if position.toMove == 1 and evaluation > finalEvaluation:
        finalEvaluation = evaluation
        finalPosition = position
      elif position.toMove == -1 and evaluation < finalEvaluation:
        finalEvaluation = evaluation
        finalPosition = position
    return finalEvaluation
    
  def generatePositions(self):
    # Iterate through all of the squares on the board
    for squareContent, x, y in self._iteratePosition():
      # Check if the resulting square contains a piece
      if _isPiece(squareContent) and squareContent * self.toMove > 0:
        # Loop through each element of the corresponding move directions array
        for move in MOVE_DIRECTIONS[squareContent]:
          for n in range(1,9):
            # Test if a move is possible, disregarding checks
            if not self._legalMove(x, y, x + move[0]*n, y + move[1]*n):
              break
            # If the move is possible, yield the position that would arise
            else:
              yield self.makeMove(x, y, x + move[0]*n, y + move[1]*n), (x, y, x + move[0]*n, y + move[1]*n)
            # If the piece is a king, a knight or a pawn (all pieces that move only once in every direction)
            # break from the loop, else, keep making the same move until it is no longer legal
            if _isKing(squareContent) or _isKnight(squareContent) or _isPawn(squareContent):
              break
    
  def makeMove(self, x, y, moveX, moveY):
    # Important variables are created
    pieceToMove = copy(self.squareArray[x][y])
    squareToMoveTo = copy(self.squareArray[moveX][moveY])
    # Copies of all relevant variables are made
    newSquareArray = deepcopy(self.squareArray)
    newCastlingRights = copy(self.castlingRights)
    newColorToMove = self.toMove * -1
    # Every move, 1 is added to the fity move proximity.
    newFiftyMoveProximity = copy(self.fiftyMoveProximity) + 1
    # If the square to move to is a piece, the fifty move proximity is reset
    if _isPiece(squareToMoveTo): newFiftyMoveProximity = 0
    # Then, exceptional moves are handled
    # If the piece to move is a king, a castling request is considered
    
    if _isKing(pieceToMove):
      # If the king wants to castle, the rook is moved in advance
      if pieceToMove < 0: # Black
        newCastlingRights[2], newCastlingRights[3] = False, False
        if x - moveX == -2: # Shorthand castling
          newSquareArray[x+1][0] = -4
          newSquareArray[0][0] = 0
        elif x - moveX == 2: # Longhand castling
          newSquareArray[x-1][0] = -4
          newSquareArray[0][0] = 0
      elif pieceToMove > 0: # White
        newCastlingRights[0], newCastlingRights[1] = False, False
        if x - moveX == -2: # Shorthand castling
          newSquareArray[x+1][7] = 4
          newSquareArray[7][7] = 0
        elif x - moveX == 2: # Longhand castling
          newSquareArray[x-1][7] = 4
          newSquareArray[0][7] = 0
      
      if _isRook(pieceToMove):
        if x == 0 and y == 0:
          newCastlingRights[3] = False
        if x == 7 and y == 0:
          newCastlingRights[2] = False
        if x == 0 and y == 7:
          newCastlingRights[1] = False
        if x == 7 and y == 7:
          newCastlingRights[0] = False

    # If a pawn is to capture en passant, the captured pawn is removed in advance
    if _isEnPassant(squareToMoveTo) and _isPawn(pieceToMove):
      if pieceToMove < 0: # Black pawn capturing a white pawn
        newSquareArray[moveX][moveY-1] = 0
      if pieceToMove > 0: # White pawn capturing a black pawn
        newSquareArray[moveX][moveY+1] = 0
    
    # The en passant square is removed (if it still exists)
    newSquareArray = _removeEnPassant(newSquareArray)

    # If the piece to move is a pawn, a double move request is considered, also, the fifty move proximity is reset
    if _isPawn(pieceToMove):
      newFiftyMoveProximity = 0
      # If a pawn wants to move forward by two squares, the en passant square is put in
      if y - moveY == -2:
        newSquareArray[x][y+1] = -7
      elif y - moveY == 2:
        newSquareArray[x][y-1] = 7

    # Finally, the piece is moved to the desired spot
    newSquareArray[x][y], newSquareArray[moveX][moveY] = 0, pieceToMove
    
    return Position(newSquareArray, newColorToMove, newFiftyMoveProximity, newCastlingRights)
    
  def __eq__(self, other):
    return self.squareArray == other.squareArray and self.toMove == other.toMove
    
  def __str__(self):
   return graphics.drawPosition(self.squareArray)

  def _kingNotInPosition(self):
    P = flattenToOneDimension(self.squareArray)
    if not 6 in P or not -6 in P:
      return True
    return False
    
  def _convertPositionToString(self):
    # Flatten the position to one dimension
    boardArray = flattenToOneDimension(self.squareArray)
    # Append the color to move to the array
    return
  
  def _iteratePosition(self):
    for x in range(8):
      for y in range(8):
        yield self.squareArray[x][y], x, y
    
  def _legalMove(self, x, y, moveX, moveY):
    # Test if the move puts the piece out of bounds
    if not -1 < moveY < 8 or not -1 < moveX < 8:
      return False
    # Initialize a variable that stores the piece type to be moved
    pieceToMove = copy(self.squareArray[x][y])
    # Initialize a  variable that stores the content of the square to be moved to
    squareToMoveTo = copy(self.squareArray[moveX][moveY])
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
      if pieceToMove < 0: # Black castling
        if x-moveX == 2: # Short castling
          if not self.squareArray[5][0] == 0 or not self.squareArray[6][0] == 0 or not self.castlingRights[2]:
            return False
        if x-moveX == -2: # Long castling
          if not self.squareArray[1][0] == 0 or not self.squareArray[2][0] == 0 or not self.squareArray[3][0] == 0 or not self.castlingRights[3]:
            return False
      elif pieceToMove > 0: # White castling
        if x-moveX == 2: # Short castling
          if not self.squareArray[5][7] == 0 or not self.squareArray[6][7] == 0 or not self.castlingRights[0]:
            return False
        if x-moveX == -2: # Long castling
          if not self.squareArray[1][7] == 0 or not self.squareArray[2][7] == 0 or not self.squareArray[3][7] == 0 or not self.castlingRights[1]:
            return False

    # Finally, evaluate if the move doesn't land a piece on a square containing a friendly piece.
    return colorIndication <= 0
  
  def _convertToInputs(self):
    # Flatten the squareArray
    return flattenToOneDimension(self.squareArray)

def _removeEnPassant(squareArray):
  return [[0 if y == 7 or y == -7 else y for y in x] for x in squareArray]

def _isPiece(squareContent):
  return -7 < squareContent < 7
    
def _isPawn(squareContent):
  return abs(squareContent) == 1

def _isKing(squareContent):
  return abs(squareContent) == 6

def _isEnPassant(squareContent):
  return abs(squareContent) == 7

def _isRook(squareContent):
  return abs(squareContent) == 4

def _isKnight(squareContent):
  return abs(squareContent) == 2
    
def flattenToOneDimension(twoDimensionalArray):
  # Take a two dimensional array and zip all of the internal lists together to form one one dimensional array
  return [y for x in twoDimensionalArray for y in x]
