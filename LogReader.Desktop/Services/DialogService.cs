using System.Threading.Tasks;
using Avalonia.Controls.ApplicationLifetimes;
using LogReader.Desktop.Contracts.Services;
using MsBox.Avalonia;

namespace LogReader.Desktop.Services;

public class DialogService : IDialogService
{
    
    private readonly IClassicDesktopStyleApplicationLifetime _desktopService;

    public DialogService(IClassicDesktopStyleApplicationLifetime desktopService)
    {
        _desktopService = desktopService;
    }

    public async Task<string?> OpenFileDialogAsync(string filter)
    {
        var files = await _desktopService.MainWindow!.StorageProvider.OpenFilePickerAsync(new()
        {
            Title = "Open Text File",
            AllowMultiple = false
        });
        return files is [{ } file, ..] ? file.Path.AbsolutePath : null;
    }

    public async Task<string?> OpenFolderDialogAsync()
    {
        var folders = await _desktopService.MainWindow!.StorageProvider.OpenFolderPickerAsync(new()
        {
            Title = "Open Folder",
            AllowMultiple = false
        });
        return folders is [{ } folder, ..] ? folder.Path.AbsolutePath : null;
    }

    public async Task ShowMessage(string message, string title)
    {
        var msg = MessageBoxManager.GetMessageBoxStandard(title, message);
        await msg.ShowWindowDialogAsync(_desktopService.MainWindow);
    }
}