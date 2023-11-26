using System;
using System.IO;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using LogReader.Core.Contracts.Services;
using LogReader.Core.Services;
using LogReader.Desktop.Contracts.Services;
using LogReader.Desktop.Services;
using LogReader.Desktop.ViewModels;
using LogReader.Desktop.Views;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LogReader.Desktop;

public class App : Application
{
    private IHost _host = null!;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        // Line below is needed to remove Avalonia data validation.
        // Without this line you will get duplicate validations from both Avalonia and CT
        BindingPlugins.DataValidators.RemoveAt(0);

        
        switch (ApplicationLifetime)
        {
            case IClassicDesktopStyleApplicationLifetime desktopLifetime:
                InitializeServices(desktopLifetime.Args);
                desktopLifetime.MainWindow = _host.Services.GetRequiredService<ShellWindow>();
                break;
            case { } invalidLifetime:
                throw new InvalidOperationException($"The application is not intended to run in {invalidLifetime.GetType()} mode");
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void InitializeServices(string[]? args)
    {
        var appLocation = Path.GetDirectoryName(AppContext.BaseDirectory)!;

        _host = Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration(builder => { builder.SetBasePath(appLocation); })
            .ConfigureServices(ConfigureServices)
            .Build();

        _host.Start();
    }

    private void ConfigureServices(IServiceCollection services)
    {
        var desktop = (IClassicDesktopStyleApplicationLifetime)Current!.ApplicationLifetime!;

        // Core Services
        services.AddTransient<IFileReader, FileReader>();
        services.AddTransient<IFileUpdateNotifierFactory, FileAppendMonitorFactory>();
        services.AddTransient<ILogParser, LogParser>();

        // Desktop Services
        services.AddSingleton(desktop);
        services.AddTransient<IDirectoryViewModelFactory, DirectoryViewModelFactory>();
        services.AddTransient<IDialogService, DialogService>();
        services.AddTransient<IUserSettingsService, UserSettingsService>();

        // Views and ViewModels
        services.AddSingleton<ShellWindow>();
        services.AddSingleton<ShellViewModel>();
    }
}