using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Controls.ApplicationLifetimes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LogReader.Desktop.Contracts.Services;

namespace LogReader.Desktop.ViewModels;

/// <summary>
/// ViewModel for the main window or shell of the application.
/// </summary>
public partial class ShellViewModel : ObservableObject
{
    private readonly IClassicDesktopStyleApplicationLifetime _desktopService;
    private readonly IDialogService _dialogService;
    private readonly IDirectoryViewModelFactory _directoryViewModelFactory;

    [ObservableProperty]
    private ObservableCollection<DirectoryViewModel> _directories;

    [ObservableProperty]
    private DirectoryViewModel? _selectedDirectory;

    public ShellViewModel(
        IClassicDesktopStyleApplicationLifetime desktopService,
        IDirectoryViewModelFactory directoryViewModelFactory,
        IDialogService dialogService)
    {
        _desktopService = desktopService;
        _directoryViewModelFactory = directoryViewModelFactory;
        _dialogService = dialogService;
        Directories = new();
    }

    /// <summary>
    /// Constructor for XAML previewer with sample data.
    /// </summary>
    public ShellViewModel()
    {
        _dialogService = null!;
        _desktopService = null!;
        _directoryViewModelFactory = null!;
        Directories = new()
        {
            new(new("LogReader.Desktop"), null!),
            new(new("LogReader.Core"), null!),
            new(new("LogReader.Tests"), null!)
        };
    }

    [RelayCommand]
    public async Task OpenDirectory()
    {
        var directoryPath = await _dialogService.OpenFolderDialogAsync();
        if (string.IsNullOrEmpty(directoryPath))
        {
            return;
        }

        var directoryViewModel = _directoryViewModelFactory.CreateViewModel(directoryPath);
        Directories.Add(directoryViewModel);
        SelectedDirectory = directoryViewModel;
    }

    [RelayCommand]
    public async Task OpenFile()
    {
        var filePath = await _dialogService.OpenFileDialogAsync("*.*");
        if (string.IsNullOrEmpty(filePath))
        {
            return;
        }

        var directoryPath = Path.GetDirectoryName(filePath)!;
        var fileName = Path.GetFileName(filePath);
        var directoryViewModel = _directoryViewModelFactory.CreateViewModel(directoryPath, fileName);
        
        Directories.Add(directoryViewModel);
        SelectedDirectory = directoryViewModel;
    }

    [RelayCommand]
    public void Close(int directoryIndex)
    {
        if (directoryIndex >= 0 && directoryIndex < Directories.Count)
        {
            Directories.RemoveAt(directoryIndex);
        }
    }

    [RelayCommand]
    public void Exit()
    {
        _desktopService.MainWindow?.Close();
    }

    partial void OnSelectedDirectoryChanged(DirectoryViewModel? oldValue, DirectoryViewModel? newValue)
    {
        oldValue?.OnDeactivated();
        newValue?.OnActivated();
    }
}
