#include <iostream>
#include <Windows.h>
#include <random>
#include "Game.h"
#include "TicTacToeAI.h"

Game::Game(int pPlayerUse)
{
	// Initialize data
	mHorizontalLine = "   -";
	mLabel = "     ";

	for (int i = 0; i < GAME_SIZE; i++)
	{
		for (int j = 0; j < GAME_SIZE; j++)
		{
			mState[i][j] = 0;
		}
		mLabel += std::to_string(i) + "   ";
		mHorizontalLine += "----";
	}
	mLabel += "\n";
	mHorizontalLine += "\n";

	mAI = new TicTacToeAI(this, -pPlayerUse);
	mPlayerUse = pPlayerUse;
	mPlayerTurn = pPlayerUse > 0;
	mMovesCount = 0;

	PrintGameState();

	srand(time(0));
}

Game::~Game()
{
	if (mAI) delete mAI;
}

void Game::NextTurn()
{
	// check if game should be over
	{
		// check if someone has won
		int won = CheckWon();
		if (won != 0)
		{
			std::cout << (won == mPlayerUse ?
				"\nYou've won!\n" : "\nYou've lost...\n");
			mIsOver = true;
			return;
		}

		// check if board has been filled up
		if (mMovesCount >= GAME_SIZE * GAME_SIZE)
		{
			std::cout << "\nBoard has been filled up. It's a draw!\n";
			mIsOver = true;
			return;
		}
	}

	if (mPlayerTurn)
	{
		std::cout << "\nYour turn! Type in [row] [column]\n";
		// Wait for player input
		int inRow = -1, inCol = -1;
		while (true)
		{
			std::cin >> inRow >> inCol;
			int verify = VerifyMove(inRow, inCol);
			if (verify == 0)
			{
				// legal move
				RecordMove(inRow, inCol, true);
				break;
			}
			else if (verify == -1)
			{
				// out of bounds
				std::cout << "\nOut of bounds! Try somewhere else?\n";
			}
			else
			{
				// move already taken
				std::cout << "\nThat spot is taken... Try somewhere else?\n";
			}
		}
	}
	else
	{
		std::cout << "\nOpponent's turn.\n";
		// Wait a random time to feel like "thinking"
		Sleep(rand() % 1000 + 500);

		// AI makes a move
		mAI->MakeMove(mState);
	}

	PrintGameState();
	mPlayerTurn = !mPlayerTurn;
}

void Game::RecordMove(int pRow, int pCol, bool pIsPlayer)
{
	mState[pRow][pCol] = (pIsPlayer ? mPlayerUse : -mPlayerUse);
	mMovesCount++;
}

void Game::PrintGameState()
{
	std::cout << mLabel;
	std::cout << mHorizontalLine;

	for (int i = 0; i < GAME_SIZE; i++)
	{
		std::cout << " " << i << " |";
		
		for (int j = 0; j < GAME_SIZE; j++)
		{
			std::cout << (mState[i][j] > 0 ? " X |" : 
				((mState[i][j] < 0) ? " O |" : "   |"));
		}

		std::cout << "\n" << mHorizontalLine;
	}
}

/// <summary>
/// Checks if a move is legal
/// Return -1 = out of bounds, -2 = occupied, 0 = legal
/// </summary>
int Game::VerifyMove(int pRow, int pCol)
{
	if (pRow < 0 || pRow > GAME_SIZE - 1) return -1;
	if (pCol < 0 || pCol > GAME_SIZE - 1) return -1;

	return mState[pRow][pCol] == 0 ? 0 : -2;
}

/// <summary>
/// Returns 0 if no one has won, returns the side marker if someone has
/// </summary>
int Game::CheckWon()
{
	for (int i = 0; i < GAME_SIZE; i++)
	{
		for (int j = 0; j < GAME_SIZE; j++)
		{
			int val = mState[i][j];
			if (val == 0) continue;
			// only need to check 4 directions:
			// right, down, down-left, down-right
			bool r = true, d = true, dl = true, dr = true;
			
			for (int k = 1; k < WIN_LENGTH; k++)
			{
				if (r && !CheckValMatch(i, j + k, val)) r = false;
				if (d && !CheckValMatch(i + k, j, val)) d = false;
				if (dl && !CheckValMatch(i + k, j - k, val)) dl = false;
				if (dr && !CheckValMatch(i + k, j + k, val)) dr = false;
			}

			if (r || d || dl || dr)
			{
				return val;
			}
		}
	}

	return 0;
}

// Helper
bool Game::CheckValMatch(int pRow, int pCol, int pVal)
{
	if (pRow < 0 || pRow > GAME_SIZE - 1) return false;
	if (pCol < 0 || pCol > GAME_SIZE - 1) return false;

	return mState[pRow][pCol] == pVal;
}