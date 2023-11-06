using LogReader.Console.Contracts.Services;
using LogReader.Core.Contracts.Services;

namespace LogReader.Console.Services;

using System;

public class ConsoleService : IConsoleService
{
    private readonly IFileService _fileService;
    private readonly IDirectoryService _directoryService;

    public ConsoleService(IFileService fileService, IDirectoryService directoryService)
    {
        _fileService = fileService;
        _directoryService = directoryService;
    }

    public async Task RunAsync(string[] args)
    {
        if (args is not [{ } directoryPath])
        {
            Console.WriteLine("Error: The directory path cannot be empty. Please enter a directory path.");
            return;
        }

        var fileName = ChooseFile(directoryPath);
        if (fileName is null)
        {
            return;
        }
        
        Console.WriteLine($"Listing log records from {fileName}:");
        await OpenFile(fileName);
    }

    private string? ChooseFile(string directoryPath)
    {
        var directory = _directoryService.TryLoad(directoryPath);
        if (directory is null)
        {
            Console.WriteLine($"Error: Directory \"{directoryPath}\" does not exist or cannot be accessed. " +
                              $"Please check the directory path and try again.");
            return null;
        }

        Console.WriteLine("Files in the directory:");
        for (var i = 0; i < directory.FileNames.Count; i++)
        {
            Console.WriteLine($"#{i+1,-3} {directory.FileNames[i],10}");
        }
        
        Console.Write("Enter file number to open: ");
        var input = Console.ReadLine();
        if (!int.TryParse(input, out var number) || number < 1 || number > directory.FileNames.Count)
        {
            Console.WriteLine("Invalid number.");
            return null;
        }

        return directory.FileNames[number - 1];
    }

    private async Task OpenFile(string fileName)
    {
        var logFile = await _fileService.TryReadAsync(fileName);
        if (logFile is null)
        {
            Console.WriteLine(
                $"Error: File \"{fileName}\" does not exist or cannot be accessed. Please check the file path and try again.");
            return;
        }

        Console.CancelKeyPress += (_, _) =>
        {
            ClearLine();
            Console.WriteLine("Exit triggered by Ctrl+C.");
            Environment.Exit(0);
        };

        for (var i = 0; i < logFile.Records.Count; i++)
        {
            var record = logFile.Records[i];
            Console.WriteLine(record.FullDetails);
            Console.WriteLine(new string('-', 80));

            if (i < logFile.Records.Count - 1)
            {
                Console.Write("Press any key to display the next log record, or Ctrl+C to exit.");
                ReadKey();
            }

            ClearLine();
        }

        Console.WriteLine("No more log records to display.");
    }

    private static void ClearLine()
    {
        if (Console.IsOutputRedirected)
        {
            return;
        }
        Console.SetCursorPosition(0, Console.CursorTop);
        Console.Write(new string(' ', Console.WindowWidth));
        Console.SetCursorPosition(0, Console.CursorTop);
    }

    private static void ReadKey()
    {
        if (Console.IsOutputRedirected)
        {
            Console.ReadLine();
            Console.ReadLine();
        }
        else
        {
            Console.ReadKey();
        }
    }
}
