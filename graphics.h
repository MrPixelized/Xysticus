#include <unordered_map>
#include <iostream>

using namespace std;

unordered_map<int, char> PIECE_REPRESENTATIONS({
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
});

unordered_map<string, char> BORDER_GRAPHICS({
});

string drawPosition(int squareArray[]):
  // If we add the top line, the middle part, and the bottom line, we get the board.
  return _printTopLine() + "\n" + _printMiddlePart(squareArray) + "\n" + _printBottomLine() 
 
int main() {
	
	cout << '\u250C';
	for (auto dingus : PIECE_REPRESENTATIONS) {
		cout << dingus.second;
	}	
	return 0;
}