#pragma once
#define AI_DEPTH 5

#include "Game.h"
#include <vector>

class TicTacToeAI
{
public:
	TicTacToeAI(Game* pGame, int pAIUse);
	~TicTacToeAI();

	void Initialize();
	void MakeMove(int (&pState)[GAME_SIZE][GAME_SIZE]);

private:
	Game* mGame;

	int mAIUse;
	float EvaluateState(int (&pState)[GAME_SIZE][GAME_SIZE],
		std::vector<std::pair<int, int> >& pAvailable, 
		int pTurn, int pMovesLeft, int pDepth);
};