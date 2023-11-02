using LogReader.Console.Contracts.Services;
using LogReader.Core.Contracts.Services;

namespace LogReader.Console.Services;

using System;

public class ConsoleService : IConsoleService
{
    private readonly ILogFileService _logFileService;

    public ConsoleService(ILogFileService logFileService)
    {
        _logFileService = logFileService;
    }

    public async Task RunAsync(string[] args)
    {
        if (args is not [{ } fileName])
        {
            Console.WriteLine("Error: The file name cannot be empty. Please enter a file name.");
            return;
        }
        
        var logFile = await _logFileService.TryReadAsync(fileName);
        if (logFile is null)
        {
            Console.WriteLine($"Error: File \"{fileName}\" does not exist or cannot be accessed. Please check the file path and try again.");
            return;
        }

        Console.CancelKeyPress += (_, _) => {
            ClearLine(false);
            Console.WriteLine("Exit triggered by Ctrl+C.");
            Environment.Exit(0);
        };

        for (var i = 0; i < logFile.Records.Count; i++)
        {
            Console.WriteLine(logFile.Records[i].Text);
            Console.WriteLine(new string('-', 80));

            if (i < logFile.Records.Count - 1)
            {
                Console.Write("Press any key to display the next log record, or Ctrl+C to exit.");
                var input = Console.ReadLine();
                if (input == null) // Ctrl+C
                {
                    return;
                }
            }

            ClearLine(true);
        }

        Console.WriteLine("No more log records to display.");

        static void ClearLine(bool oneLineBack)
        {
            if (Console.IsOutputRedirected)
            {
                return;
            }
            Console.SetCursorPosition(0, Console.CursorTop - (oneLineBack ? 1 : 0));
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, Console.CursorTop);
        }
    }
}
