 using System;
 using System.Collections.Generic;
 
 const sbyte[,] STANDARD_POSITION = new int[8,8] {
  {-4,-1,0,0,0,0,1,4},
  {-2,-1,0,0,0,0,1,2},
  {-3,-1,0,0,0,0,1,3},
  {-5,-1,0,0,0,0,1,5},
  {-6,-1,0,0,0,0,1,6},
  {-3,-1,0,0,0,0,1,3},
  {-2,-1,0,0,0,0,1,2},
  {-4,-1,0,0,0,0,1,4}
};

const Dictionary<sbyte, sbyte> MOVE_DIRECTIONS = new Dictionary<sbyte,sbyte>() {
  {-6, {(1,1),(-1,1),(-1,-1),(1,-1),(0,1),(0,-1),(1,0),(-1,0),(2,0),(-2,0)}},
  {-5, {(1,1),(-1,1),(-1,-1),(1,-1),(0,1),(0,-1),(1,0),(-1,0)}},
  {-4, {(0,1),(0,-1),(1,0),(-1,0)}},
  {-3, {(1,1),(-1,1),(-1,-1),(1,-1)}},
  {-2, {(1,2),(-1,2),(1,-2),(-1,-2)}},
  {-1, {(0,1),(1,1),(-1,1),(0,2)}},
  { 1, {(0,-1),(1,-1),(-1,-1),(0,-2)}},
  { 2, {(1,2),(-1,2),(1,-2),(-1,-2)}},
  { 3, {(1,1),(-1,1),(-1,-1),(1,-1)}},
  { 4, {(0,1),(0,-1),(1,0),(-1,0)}},
  { 5, {(1,1),(-1,1),(-1,-1),(1,-1),(0,1),(0,-1),(1,0),(-1,0)}},
  { 6, {(1,1),(-1,1),(-1,-1),(1,-1),(0,1),(0,-1),(1,0),(-1,0),(2,0),(-2,0)}}
};

const bool[] STANDARD_CASTLING_RIGHTS = new bool[4] {true, true, true};

public class Position {
	public int toMove;
	public int fiftyMoveProximity;
	public int[8,8] board;
	public bool[4] castlingRights;
	
	public Position(int[8,8] position = STANDARD_POSITION, int toMove = 1, int fiftyMoveProximity = 0, bool[] castlingRights = STANDARD_CASTLING_RIGHTS {
		board = position;
		toMove = toMove;
		fiftyMoveProximity = fiftyMoveProximity;
		castlingRights = castlingRights;
	}
	
	public float findBestMove(int depth, float alpha, float beta) {
	
}
