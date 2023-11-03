﻿using LogReader.Console.Contracts.Services;
using LogReader.Console.Services;
using LogReader.Core.Contracts.Services;
using LogReader.Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace LogReader.Console;

public class Program
{
    public static async Task Main(string[] args)
    {
        var serviceProvider = ConfigureServices();
        var consoleService = serviceProvider.GetRequiredService<IConsoleService>();
        await consoleService.RunAsync(args);
    }
    
    public static IServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        services.AddSingleton<ILogFileService, LogFileService>();
        services.AddSingleton<IConsoleService, ConsoleService>();

        return services.BuildServiceProvider();
    }
}