#include "GameManager.h"
#include "Game.h"

GameManager* GameManager::mInstance;

GameManager::GameManager()
	: mSessionIsOver(false)
{

}

GameManager::~GameManager()
{

}

Game* GameManager::StartNewGame()
{
	if (mGame)
	{
		delete mGame;
	}

	std::cout << "\nChoose your side: X goes first (x/o)\n";
	char input;
	std::cin >> input;

	if (input == 'x')
	{
		mGame = new Game(1);
	}
	else
	{
		mGame = new Game(-1);
	}

	return mGame;
}

Game* GameManager::GetCurrentGame()
{
	return mGame;
}

void GameManager::Step()
{
	if (!mGame)
	{
		StartNewGame();
		return;
	}

	// if current game is over, prompt to start a new one
	if (mGame->IsOver())
	{
		std::cout << "\nPlay again? (y/n)\n";
		char input;
		std::cin >> input;

		if (input == 'y')
		{
			StartNewGame();
			return;
		}
		else
		{
			std::cout << "\nIt was fun, thanks for playing!\n";
			EndSession();
			return;
		}
	}

	// If game is still going, move to next turn
	mGame->NextTurn();
}

void GameManager::EndSession()
{
	mSessionIsOver = true;
}