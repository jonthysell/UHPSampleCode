using System;

namespace SampleEngine
{
    class Program
    {
        static void Main(string[] args)
        {
            var engine = new Engine(Console.WriteLine);

            engine.Start();

            string? line;
            while (!engine.ExitRequested)
            {
                line = Console.ReadLine() ?? "";
                engine.ReadLine(line);
            }
        }
    }
}
