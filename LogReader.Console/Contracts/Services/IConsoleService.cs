namespace LogReader.Console.Contracts.Services;

public interface IConsoleService
{
    Task RunAsync(string[] args);
}