// TicTacToe.cpp : This file contains the 'main' function. Program execution begins and ends there.
//

#include <iostream>
#include "GameManager.h"


int main()
{
    // Initialize game
    GameManager* mManager = GameManager::Instance();

    // Game loop
    while (!mManager->SessionIsOver())
    {
        mManager->Step();
    }

    // Clean up
    GameManager::DestroyInstance();

    return 0;
}
