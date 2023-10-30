using System.Threading.Tasks;

using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using LogReader.Core.Contracts.Services;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;

namespace LogReader.Desktop.ViewModels;

public partial class ShellViewModel : ObservableObject
{
    private readonly ILogFileService _logFileService;
    private readonly IClassicDesktopStyleApplicationLifetime _desktopService;

    [ObservableProperty]
    private LogViewModel? _logViewModel;

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
        var fileName = files[0].TryGetLocalPath()!;
        if (!_logFileService.TryRead(fileName, out var logs))
        {
            var msg = MessageBoxManager.GetMessageBoxStandard(
                "Open Text File",
                $"{fileName}\r\nFile not found.\r\nCheck the file name and try again.",
                ButtonEnum.Ok,
                Icon.Warning);
            await msg.ShowWindowDialogAsync(_desktopService.MainWindow);
            return;
        }

        LogViewModel = new(logs);
    }

    [RelayCommand]
    public void Exit()
    {
        _desktopService.MainWindow?.Close();
    }
}
