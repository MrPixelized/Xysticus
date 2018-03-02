import time
import position
from defs import _turnedBoard, ENGINE_NAME

INVERSED_PIECE_REPRESENTATIONS = {
  "P":1,
  "N":2,
  "B":3,
  "R":4,
  "Q":5,
  "K":6,
  "p":-1,
  "n":-2,
  "b":-3,
  "r":-4,
  "q":-5,
  "k":-6
}
COORDINATE_TRANSFORMATION = {
  "a":0,
  "b":1,
  "c":2,
  "d":3,
  "e":4,
  "f":5,
  "g":6,
  "h":7
}

def fenParser(FEN):
  # The FEN string is split into different parts, all separated by spaces.
  FEN = FEN.split(" ")
  # The first part of the FEN string represents the board.
  squareArray = []
  # Setting up the board, going rank by rank (as that's how FEN strings are made up).
  for rank in FEN[0].split("/"):
    rankList = []
    for char in rank:
      if _isNumber(char):
        # If a number appears in the board representation, that number times empty space is added to the board.
        rankList += int(char) * [0]
      else:
        # Otherwise, the number representing the pice is added to the board.
        rankList.append(INVERSED_PIECE_REPRESENTATIONS[char])
    squareArray.append(rankList)
  # Turning the board around so it fits the interal board representation.
  squareArray = _turnedBoard(squareArray)

  # The second part of FEN string is the side to move.
  if FEN[1] == "w":
    toMove = 1
  else:
    toMove = -1

  # The third part of the FEN string specifies which castling rights are still in the game.
  castlingRights = [False, False, False, False]
  if "K" in FEN[2]: castlingRights[0] = True
  if "Q" in FEN[2]: castlingRights[1] = True
  if "k" in FEN[2]: castlingRights[2] = True
  if "q" in FEN[2]: castlingRights[3] = True

  # The fourth part of the FEN string specifies the en passant square.
  if not FEN[3] == "-":
    enPassantCoordinates = COORDINATE_TRANSFORMATION[FEN[3][0]], 8 - int(FEN[3][1])
    squareArray[enPassantCoordinates[0]][enPassantCoordinates[1]] = -7 if toMove == 1 else 7

  # Setting the fifty move proximity.
  fiftyMoveProximity = int(FEN[4])

  # Setting the move number.
  move = int(FEN[5])

  return squareArray, toMove, castlingRights, fiftyMoveProximity, move

def _isNumber(arg):
  try:
    int(arg)
    return True
  except ValueError:
    return False

def _inputUCI():
	print("id name %s" % (ENGINE_NAME))
	print("id author Ischa Abraham, Jeroen van den Berg")
	print("uciok")

def _inputIsReady():
	print("readyok")

def _inputSetOption(strInput):
	pass

def _inputUCINewGame():
	pass

def _inputPosition():
	pass

def _inputGo():
	pass

def _inputStop():
	pass	

def _inputQuit():
	pass

def _inputPrint():
	pass

def _inputUnknown(strInput):
	print("Unknown command: %s" % (strInput))

currTime = time.time()
print(fenParser("r3kb1r/pp3ppp/2q2n2/2pp4/Q2P4/4P3/PP3PPP/RNB1K2R w KQkq - 0 11"))
print(time.time() - currTime)

a = position.Position()

while True:
	strInput = input().split(" ")
	if strInput[0] == "uci":
		_inputUCI()
	elif strInput[0] == "isready":
		_inputIsReady()
	elif strInput[0] == "setoption":
		_inputSetOption(strInput[1:])
	elif strInput[0] == "ucinewgame":
		_inputUCINewGame()
	elif strInput[0] == "position":
		_inputPosition()
	elif strInput[0] == "go":
		_inputGo()
	elif strInput[0] == "stop":
		_inputStop()
	elif strInput[0] == "quit":
		_inputQuit()
	elif strInput[0] == "print":
		_inputPrint()
	else:
		_inputUnknown(strInput[0])