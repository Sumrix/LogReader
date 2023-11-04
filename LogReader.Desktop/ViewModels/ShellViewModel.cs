using System.Threading.Tasks;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LogReader.Core.Contracts.Services;
using LogReader.Core.Helpers;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;

namespace LogReader.Desktop.ViewModels;

public partial class ShellViewModel : ObservableObject
{
    private readonly ILogFileService _logFileService;
    private readonly IClassicDesktopStyleApplicationLifetime _desktopService;
    private string? _fileName;

    [ObservableProperty]
    private string _title = "Log Reader";

    [ObservableProperty]
    private LogFileViewModel? _logFileViewModel;

    public ShellViewModel(ILogFileService logFileService, IClassicDesktopStyleApplicationLifetime desktopService)
    {
        _logFileService = logFileService;
        _desktopService = desktopService;
    }

    [RelayCommand]
    public async Task Open()
    {
        var files = await _desktopService.MainWindow!.StorageProvider.OpenFilePickerAsync(new()
        {
            Title = "Open Text File",
            AllowMultiple = false
        });
        _fileName = files[0].TryGetLocalPath()!;
        Title = $"Log Reader - {_fileName.TruncateLeft(60, true)}";

        await ReloadLogFile();
    }

    [RelayCommand]
    public void Exit()
    {
        _desktopService.MainWindow?.Close();
    }

    [RelayCommand]
    public async Task Refresh()
    {
        await ReloadLogFile();
    }

    private async Task ReloadLogFile()
    {
        if (_fileName is null)
        {
            return;
        }

        var logFile = await _logFileService.TryReadAsync(_fileName);
        if (logFile is null)
        {
            var msg = MessageBoxManager.GetMessageBoxStandard(
                "Open Text File",
                $"{_fileName}\r\nFile not found.\r\nCheck the file name and try again.",
                ButtonEnum.Ok,
                Icon.Warning);
            await msg.ShowWindowDialogAsync(_desktopService.MainWindow);
            return;
        }

        LogFileViewModel = new(logFile);
    }
}
