#include <unordered_map>
#include <iostream>
#include <string>

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

unordered_map<string, wchar_t> BORDER_GRAPHICS({
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
});

/*
string drawPosition(int squareArray[]):
  // If we add the top line, the middle part, and the bottom line, we get the board.
  return _printTopLine() + "\n" + _printMiddlePart(squareArray) + "\n" + _printBottomLine() 
 
string _printTopLine() {
  // Printing the top line of the board, which is something like this: ┌─┬─┬─┬─┬─┬─┬─┬─┐
  // return BORDER_GRAPHICS["top left corner"] + 7 * (BORDER_GRAPHICS["horizontal"] + BORDER_GRAPHICS["top split edge"]) + BORDER_GRAPHICS["horizontal"] + BORDER_GRAPHICS["top right corner"]
  return 7 * 'd';
}
*/
 
int main() {
    setlocale(LC_ALL, "");
    // _printTopLine();
	cout << (wchar_t)(U'\u2780');
	for (auto dingus : PIECE_REPRESENTATIONS) {
		cout << dingus.second;
	}	
	return 0;
}
