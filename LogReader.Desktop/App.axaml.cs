using System.IO;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using LogReader.Core.Contracts.Services;
using LogReader.Core.Services;
using LogReader.Desktop.ViewModels;
using LogReader.Desktop.Views;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LogReader.Desktop;

public class App : Application
{
    private IHost? _host;

    public T? GetService<T>()
        where T : class
        => _host?.Services.GetService(typeof(T)) as T;
    
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
            case IClassicDesktopStyleApplicationLifetime desktop:
                StartHost(desktop.Args);
                desktop.MainWindow = GetService<ShellWindow>();
                break;
            //case { } invalidLifetime:
            //    throw new InvalidOperationException($"The application is not intended to run in {invalidLifetime.GetType()} mode");
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void StartHost(string[]? args)
    {
        var appLocation = Path.GetDirectoryName(System.AppContext.BaseDirectory)!;

        _host = Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration(c => { c.SetBasePath(appLocation); })
            .ConfigureServices(ConfigureServices)
            .Build();

        _host.Start();
    }

    private void ConfigureServices(HostBuilderContext context, IServiceCollection services)
    {
        var desktop = (IClassicDesktopStyleApplicationLifetime)Current!.ApplicationLifetime!;

        // Core Services
        services.AddSingleton<IFileService, FileService>();

        // Services
        services.AddSingleton(desktop);

        // Views and ViewModels
        services.AddTransient<ShellWindow>();
        services.AddTransient<ShellViewModel>();

        services.AddTransient<FileViewModel>();

        // Configuration
    }
}