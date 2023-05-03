#pragma once
#define GAME_SIZE 3
#define WIN_LENGTH 3

#include <string>

class Game
{
public:
	Game(int pPlayerUse);
	~Game();

	// Go into the next turn;
	void NextTurn();

	// Register a move
	void RecordMove(int pRow, int pCol, bool pIsPlayer);

	// Helper for checking if someone has won
	int CheckWon();

	bool IsOver() { return mIsOver; }

private:
	int mPlayerUse;
	bool mPlayerTurn;

	bool mIsOver;

	class TicTacToeAI* mAI;

	int mState[GAME_SIZE][GAME_SIZE];
	int mMovesCount;

	int VerifyMove(int pRow, int pCol);
	void PrintGameState();
	bool CheckValMatch(int pRow, int pCol, int pVal);

	std::string mHorizontalLine;
	std::string mLabel;
};
