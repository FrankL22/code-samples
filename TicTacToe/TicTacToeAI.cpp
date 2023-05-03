#include "TicTacToeAI.h"

TicTacToeAI::TicTacToeAI(Game* pGame, int pAIUse)
	: mGame(pGame),
	mAIUse(pAIUse)
{
	Initialize();
}

TicTacToeAI::~TicTacToeAI()
{
	
}

void TicTacToeAI::Initialize()
{

}

void TicTacToeAI::MakeMove(int(&pState)[GAME_SIZE][GAME_SIZE])
{
	std::vector<std::pair<int, int> > available;
	for (int i = 0; i < GAME_SIZE; i++)
	{
		for (int j = 0; j < GAME_SIZE; j++)
		{
			if (pState[i][j] == 0)
			{
				available.push_back(std::make_pair(i, j));
			}
		}
	}

	int optionsCount = available.size();
	float bestScore = -10000.0f;
	std::pair<int, int> bestOption = std::make_pair(-1, -1);
	for (int i = 0; i < optionsCount; i++)
	{
		std::pair<int, int> curr = available[i];

		pState[curr.first][curr.second] = mAIUse;
		float score = EvaluateState(pState, available, -mAIUse, optionsCount - 1, AI_DEPTH - 1);
		if (bestScore < score)
		{
			bestScore = score;
			bestOption = curr;
		}
		pState[curr.first][curr.second] = 0;
	}

	mGame->RecordMove(bestOption.first, bestOption.second, false);
}

float TicTacToeAI::EvaluateState(int(&pState)[GAME_SIZE][GAME_SIZE], 
	std::vector<std::pair<int, int> > &pAvailable, 
	int pTurn, int pMovesLeft, int pDepth)
{
	int won = mGame->CheckWon();
	if (won == mAIUse)
	{
		// if it's a won state, evaluate to 1
		return 1.0f;
	}
	if (won == -mAIUse)
	{
		// if it's a lost state, evaluate to -1
		return -1.0f;
	}

	if (pMovesLeft < 1 || pDepth < 1)
	{
		// if it's a draw state or search depth has been exhausted, evaluate to 0
		return 0.0f;
	}

	// if it's a neutral state, evaluate to the average of substates
	float sum = 0.0f;
	int count = 0;
	for (int i = 0; i < pAvailable.size(); i++)
	{
		std::pair<int, int> curr = pAvailable[i];
		if (pState[curr.first][curr.second] != 0) continue;

		count++;
		pState[curr.first][curr.second] = pTurn;
		sum += EvaluateState(pState, pAvailable, -pTurn, pMovesLeft - 1, pDepth - 1);
		pState[curr.first][curr.second] = 0;
	}
	return sum / count;
}