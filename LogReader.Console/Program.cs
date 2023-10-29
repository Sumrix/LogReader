using LogReader.Console.Contracts.Services;
using LogReader.Console.Services;
using LogReader.Core.Contracts.Services;
using LogReader.Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace LogReader.Console;

public class Program
{
    public static void Main(string[] args)
    {
        var serviceProvider = ConfigureServices();
        var consoleService = serviceProvider.GetRequiredService<IConsoleService>();
        consoleService.Run(args);
    }
    
    public static IServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        services.AddSingleton<ILogFileService, LogFileService>();
        services.AddSingleton<IConsoleService, ConsoleService>();

        return services.BuildServiceProvider();
    }
}