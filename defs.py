ENGINE_NAME = "Xysticus"

def turnedBoard(squareArray):
  return ([[y[x] for y in squareArray] for x in range(8)])
