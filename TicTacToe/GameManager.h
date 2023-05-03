#pragma once
#include <iostream>

// Singleton game manager class
class GameManager
{
public:
	static GameManager* Instance()
	{
		if (mInstance == nullptr)
		{
			mInstance = new GameManager();
		}
		return mInstance;
	}

	static void DestroyInstance()
	{
		if (mInstance != nullptr)
		{
			delete mInstance;
		}
	}

	GameManager();
	~GameManager();

	class Game* StartNewGame();
	class Game* GetCurrentGame();

	// Step into the next action
	void Step();

	bool SessionIsOver() { return mSessionIsOver; }
	void EndSession();

private:
	static GameManager* mInstance;

	bool mSessionIsOver;
	class Game* mGame;
};

