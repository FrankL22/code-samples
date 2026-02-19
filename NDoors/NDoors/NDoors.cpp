// NDoors.cpp : This file contains the 'main' function. Program execution begins and ends there.
//

#include <iostream>
#include <algorithm>
#include <random>
#include <chrono>
#include <fstream>
#include <ctime>
#include <sstream>
#include <iomanip>

/// <param name="low">lower bound, inclusive</param>
/// <param name="high">highe bound, inclusive</param>
int helper_numWrap(int num, int low, int high)
{
    if (num >= low && num <= high) return num;
    if (num < low) return helper_numWrap(high - (low - num - 1), low, high);
    if (num > high) return helper_numWrap(low + (num - high - 1), low, high);
}

/// <summary>
/// prints progress bar in format: Running simulations currProgress/totalCount >>>>>>>>>>>>........
/// </summary>
void helper_printProgress(int barWidth, int currProgress, int totalCount)
{
    std::string progress = "\rRunning simulations: " + std::to_string(currProgress) + "/" + std::to_string(totalCount);

    int completedWidth = (int)((float) barWidth * (float)currProgress / (float)totalCount);
    std::string bar = " [";
    for (int i = 0; i < completedWidth; i++)
    {
        bar += ">";
    }
    for (int j = completedWidth; j < barWidth; j++)
    {
        bar += ".";
    }
    bar += "]";
    std::cout << progress << bar;
    std::cout.flush();
}

/// <param name="n">number of doors</param>
/// <param name="m">number of prizes</param>
/// <param name="x">number of removals by the game host</param>
/// <returns>1 if prize won through random selection, 0 if not won</returns>
int RunSingleSimulation(int n, int m, int x, std::string& log, int simNum, bool needLog)
{
    if (needLog) log += "@@ Sim #" + std::to_string(simNum) + " @@\n";
    // create a representation of doors for random selection
    int* rep = new int[n];
    for (int i = 0; i < n; i++)
    {
        rep[i] = i;
    }

    // apply randomness through shuffle
    std::shuffle(rep, rep + n, std::default_random_engine(std::chrono::system_clock::now().time_since_epoch().count()));

    // set up random device
    std::random_device rd;
    std::mt19937 mt(rd());
    std::uniform_int_distribution<int> distribution(0, n - 1);

    // create an account for prizes and removals
    int* acc = new int[n];
    for (int i = 0; i < n; i++)
    {
        acc[i] = 0;
    }
    // randomly assign prizes
    if (needLog) log += "prize(s): ";
    int prizeRemaining = m;
    int prizeStart = distribution(mt);
    for (int i = 0; i < m; i++)
    {
        int index = helper_numWrap(prizeStart + i, 0, n - 1);
        acc[index] = 1;
        if (needLog) log += std::to_string(index) + " ";
    }
    if (needLog) log += ";\n";

    // randomly make a first pick
    int firstPick = distribution(mt);
    if (needLog)
    {
        log += "first pick: " + std::to_string(firstPick)
            + (acc[firstPick] == 1 ? ", had prize;\n" : ";\n");
    }

    // game host randomly removes x doors
    if (needLog) log += "door(s) removed: ";
    int removeStart = distribution(mt);
    /// index is random position in rep, not door number
    int index = helper_numWrap(removeStart, 0, n - 1) - 1;
    for (int i = 0; i < x; i++)
    {
        index = helper_numWrap(index + 1, 0, n - 1);
        while (rep[index] == firstPick ||   // Cannot remove the picked door
            (/*prizeRemaining < 2 &&*/ acc[rep[index]] == 1)) // Cannot remove a door with a prize
        {
            // If current removal is illegal, skip to next index until legal
            index = helper_numWrap(index + 1, 0, n - 1);
        }

        // assumes index is now a legal removal
        acc[rep[index]] = -1;
        if (acc[rep[index]] == 1) prizeRemaining--;
        if (needLog) log += std::to_string(rep[index]) + " ";
    }
    if (needLog) log += ";\n";

    // now swap to another random door that's not removed
    int secondPick = distribution(mt);
    while (secondPick == firstPick || // Cannot pick the same pick
        acc[secondPick] < 0) // Cannot pick a removed door
    {
        // skip to next door until legal
        secondPick = helper_numWrap(secondPick + 1, 0, n - 1);
    }
    if (needLog)
    {
        log += "second pick: " + std::to_string(secondPick)
            + (acc[secondPick] > 0 ? ", HIT!\n\n" : ", no hit.\n\n");
    }

    // check if second pick is a hit
    int rtnVal = (acc[secondPick] > 0) ? 1 : 0;
    delete[] rep;
    delete[] acc;
    return rtnVal;
}


/// <param name="n">number of doors</param>
/// <param name="m">number of prizes</param>
/// <param name="x">number of removals by the game host</param>
/// <param name="k">number of simulations</param>
/// <returns>Simulated probability through the Monte-Carlo method</returns>
float RunSimulations(int n, int m, int x, int k, bool needLog)
{
    // Run params legitimacy check, as per game rules
    if (n < 3) // Cannot have less than 3 doors
    {
        std::cout << "ERROR: Cannot have less than 3 doors.\n";
        return -1.0f;
    }
    if (m > n) // Cannot have more prizes than doors
    {
        std::cout << "ERROR: Cannot have more prizes than doors.\n";
        return -1.0f;
    }
    if (x > n - m - 1) // Game host might run out of empty doors to remove
    {
        std::cout << "ERROR: With these params, the game host might run out of empty doors to remove.\n"
            << "Game host can remove at most { k = DoorCount - PrizeCount - 1 } doors.\n";
        return -1.0f; 
    }
    if (m < 1 || x < 1 || k < 1)
    {
        std::cout << "ERROR: PrizeCount, RemovalCount, and num of simulations cannot be less than 1.\n";
        return -1.0f; // Numbers validity check
    }

    std::string logText;
    if (needLog)
    {
        // Log simulation basic info
        logText = "Running " + std::to_string(k) + " simulations for " + std::to_string(n)
            + " doors, " + std::to_string(m) + " prizes, and " + std::to_string(x)
            + " removals by the game host.\n";
    }

    // Show progress bar
    int barWidth = 20;
    helper_printProgress(barWidth, 0, k);

    int hitCount = 0;
    for (int i = 0; i < k; i++)
    {
        hitCount += RunSingleSimulation(n, m, x, logText, i + 1, needLog);

        // Update progress bar
        helper_printProgress(barWidth, i + 1, k);
    }

    if (needLog)
    {
        // Open new txt file for log
        std::time_t now = std::time(nullptr);
        struct tm timeinfo;
        errno_t result = localtime_s(&timeinfo, &now); // Use localtime_s on Windows for safety
        std::ostringstream oss;
        oss << std::put_time(&timeinfo, "%H_%M_%S"); // Format using put_time
        std::string time_str = oss.str();
        std::string filename = "log_" + time_str + ".txt";
        std::ofstream logFile(filename);

        // Check if the file was opened successfully
        if (!logFile.is_open()) {
            std::cout << "Couldn't open log file " << filename << std::endl;
            return -1.0f;
        }

        // Write log
        logFile << logText;
        logFile.close();
    }
    
    return (float)hitCount / (float)k;
}

int main()
{
    int n, m, x, k, l;
    std::cout << "Run the Monte-Carlo method simulation for an N-Doos problem:\nInput num of doors, num of prizes, num of removals by game host, num of simulations, need log (0/1)\n";

    while (true)
    {
        std::cin >> n >> m >> x >> k >> l;
        std::cout << "\n";
        float prob = RunSimulations(n, m, x, k, (l == 1));
        float oldProb = (float)m / (float)n;
        if (prob < 0.0f)
        {
            std::cout << "\nSimulation couldn't be properly completed due to an error. See console log for more info.\n";
        }
        else
        {
            std::cout << "\nA run of " << k << " simulations yielded a probability of " << prob * 100 << "% to land a prize if you swap.\n";
            std::cout << "This would've been " << oldProb * 100 << "% if you don't swap, which makes you " << (prob > oldProb ? "better off." : "worse off.\n");
        }
        std::cout << "\n\nInput num of doors, num of prizes, num of removals by game host, num of simulations.\n";
    }
    
}
