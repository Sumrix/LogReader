using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Controls.ApplicationLifetimes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LogReader.Desktop.Contracts.Services;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;

namespace LogReader.Desktop.ViewModels;

public partial class ShellViewModel : ObservableObject
{
    private readonly IClassicDesktopStyleApplicationLifetime _desktopService;
    private readonly IDirectoryViewModelFactory _directoryViewModelFactory;

    [ObservableProperty]
    private ObservableCollection<DirectoryViewModel> _directories;

    [ObservableProperty]
    private DirectoryViewModel? _selectedDirectory;

    public ShellViewModel(
        IClassicDesktopStyleApplicationLifetime desktopService,
        IDirectoryViewModelFactory directoryViewModelFactory)
    {
        _desktopService = desktopService;
        _directoryViewModelFactory = directoryViewModelFactory;
        Directories = new();
    }

    // For xaml previewer
    public ShellViewModel()
    {
        _desktopService = null!;
        _directoryViewModelFactory = null!;
        Directories = new()
        {
            new(new("my/first/long/path", Array.Empty<string>()), null!),
            new(new("my/second/long/path", Array.Empty<string>()), null!),
            new(new("my/third/long/path", Array.Empty<string>()), null!)
        };
    }

    [RelayCommand]
    public async Task OpenDirectory()
    {
        var folders = await _desktopService.MainWindow!.StorageProvider.OpenFolderPickerAsync(new()
        {
            Title = "Open Folder",
            AllowMultiple = false
        });
        var folderName = folders[0].Path.AbsolutePath;

        var directoryViewModel = _directoryViewModelFactory.TryCreateViewModel(folderName);
        if (directoryViewModel is null)
        {
            var msg = MessageBoxManager.GetMessageBoxStandard(
                "Open Folder",
                $"{folderName}\r\nFolder not found.\r\nCheck the folder name and try again.",
                ButtonEnum.Ok,
                Icon.Warning);
            await msg.ShowWindowDialogAsync(_desktopService.MainWindow);
        }
        else
        {
            Directories.Add(directoryViewModel);
            SelectedDirectory = directoryViewModel;
        }
    }

    [RelayCommand]
    public async Task OpenFile()
    {
        var files = await _desktopService.MainWindow!.StorageProvider.OpenFilePickerAsync(new()
        {
            Title = "Open Text File",
            AllowMultiple = false
        });
        var filePath = files[0].Path.AbsolutePath;
        var directoryPath = Path.GetDirectoryName(filePath)!;
        var fileName = Path.GetFileName(filePath);
        
        var directoryViewModel = _directoryViewModelFactory.TryCreateViewModel(directoryPath);
        if (directoryViewModel is null)
        {
            var msg = MessageBoxManager.GetMessageBoxStandard(
                "Open Text File",
                $"{directoryPath}\r\nFolder not found.\r\nCheck the folder name and try again.",
                ButtonEnum.Ok,
                Icon.Warning);
            await msg.ShowWindowDialogAsync(_desktopService.MainWindow);
        }
        else
        {
            Directories.Add(directoryViewModel);
            SelectedDirectory = directoryViewModel;
            directoryViewModel.SelectedFileName = fileName;
        }
    }

    [RelayCommand]
    public void Close(int directoryIndex)
    {
        Directories.RemoveAt(directoryIndex);
    }

    [RelayCommand]
    public void Exit()
    {
        _desktopService.MainWindow?.Close();
    }
}
