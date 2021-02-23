#include <iostream>
#include <string>

#include "Engine.h"

void WriteLine(std::string line)
{
    std::cout << line << std::endl;
}

int main(int argc, char *argv[])
{
    SampleEngine::Engine engine{WriteLine};

    engine.Start();

    std::string line;
    while (!engine.ExitRequested())
    {
        std::getline(std::cin, line);
        engine.ReadLine(line);
    }

    return 0;
}
