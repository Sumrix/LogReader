using LogReader.Console.Contracts.Services;
using LogReader.Core.Contracts.Services;

namespace LogReader.Console.Services;

public class ConsoleService : IConsoleService
{
    private readonly ILogFileService _logFileService;

    public ConsoleService(ILogFileService logFileService)
    {
        _logFileService = logFileService;
    }

    public void Run(string[] args)
    {
        if (args is not [{ } fileName])
        {
            System.Console.WriteLine("Error: The file name cannot be empty. Please enter a file name.");
            return;
        }

        if (!_logFileService.TryRead(fileName, out var fileContent))
        {
            System.Console.WriteLine($"Error: File \"{fileName}\" does not exist or cannot be accessed. Please check the file path and try again.");
            return;
        }

        System.Console.WriteLine(fileContent);
    }
}
