from random import random

def evaluationNetwork(position):
  inputs = position._convertToInputs()
  return random()*2 -1
