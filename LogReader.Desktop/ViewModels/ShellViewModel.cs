using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Avalonia.Controls.ApplicationLifetimes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LogReader.Core.Services;
using LogReader.Desktop.Contracts.Services;
using LogReader.Desktop.Models;

namespace LogReader.Desktop.ViewModels;

/// <summary>
/// ViewModel for the main window or shell of the application.
/// </summary>
public partial class ShellViewModel : ObservableObject, IDisposable
{
    private readonly IClassicDesktopStyleApplicationLifetime _desktopService;
    private readonly IDialogService _dialogService;
    private readonly IDirectoryViewModelFactory _directoryViewModelFactory;
    private readonly Timer _saveTimer;
    private readonly IUserSettingsService _userSettingsService;

    /// <summary>
    /// Collection of directory view models representing open directories.
    /// </summary>
    [ObservableProperty]
    private ObservableCollection<DirectoryViewModel> _directories;

    private UserSettings _savedUserSettings;

    /// <summary>
    /// The currently selected directory view model.
    /// </summary>
    [ObservableProperty]
    private DirectoryViewModel? _selectedDirectory;

    /// <summary>
    /// The user settings for the application.
    /// </summary>
    public UserSettings UserSettings { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ShellViewModel"/> class.
    /// </summary>
    /// <param name="desktopService">The application lifetime service.</param>
    /// <param name="directoryViewModelFactory">The factory for creating directory view models.</param>
    /// <param name="dialogService">The service for dialog interaction.</param>
    /// <param name="userSettingsService">The service for user settings management.</param>
    public ShellViewModel(
        IClassicDesktopStyleApplicationLifetime desktopService,
        IDirectoryViewModelFactory directoryViewModelFactory,
        IDialogService dialogService,
        IUserSettingsService userSettingsService)
    {
        _desktopService = desktopService ?? throw new ArgumentNullException(nameof(desktopService));
        _directoryViewModelFactory = directoryViewModelFactory ?? throw new ArgumentNullException(nameof(directoryViewModelFactory));
        _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
        _userSettingsService = userSettingsService ?? throw new ArgumentNullException(nameof(userSettingsService));

        Directories = new();
        UserSettings = _userSettingsService.LoadSettings();
        _savedUserSettings = UserSettings.DeepCopy();
        RestoreSettings();

        _saveTimer = new(TimeSpan.FromSeconds(5))
        {
            AutoReset = true
        };
        _saveTimer.Elapsed += SaveTimerElapsed;
        _saveTimer.Start();
    }

    /// <summary>
    /// Constructor for XAML previewer with sample data.
    /// </summary>
    public ShellViewModel()
    {
        UserSettings = new();
        _dialogService = null!;
        _desktopService = null!;
        _directoryViewModelFactory = null!;
        _userSettingsService = null!;
        _saveTimer = null!;
        _savedUserSettings = null!;
        var fileReader = new FileReader(new LogParser(), new FileAppendMonitorFactory());
        Directories = new()
        {
            new(new(@"LogReader.Desktop\Assets\"), fileReader),
            new(new("LogReader.Core"), fileReader),
            new(new("LogReader.Tests"), fileReader)
        };
    }

    /// <summary>
    /// Disposes of the resources used by this ViewModel.
    /// </summary>
    public void Dispose()
    {
        _saveTimer.Dispose();
    }

    /// <summary>
    /// Closes the directory at the specified index.
    /// </summary>
    /// <param name="directoryIndex">The index of the directory to close.</param>
    [RelayCommand]
    public void CloseDirectory(int directoryIndex)
    {
        if (directoryIndex >= 0 && directoryIndex < Directories.Count)
        {
            Directories.RemoveAt(directoryIndex);
        }
    }

    /// <summary>
    /// Closes the application.
    /// </summary>
    [RelayCommand]
    public void Exit()
    {
        _desktopService.MainWindow?.Close();
    }

    /// <summary>
    /// Called when the selected directory changes.
    /// </summary>
    /// <param name="oldValue">The previous directory.</param>
    /// <param name="newValue">The new directory.</param>
    partial void OnSelectedDirectoryChanged(DirectoryViewModel? oldValue, DirectoryViewModel? newValue)
    {
        oldValue?.OnDeactivated();
        newValue?.OnActivated();
    }

    /// <summary>
    /// Opens a directory and adds its ViewModel to the collection.
    /// </summary>
    [RelayCommand]
    public async Task OpenDirectory()
    {
        var directoryPath = await _dialogService.OpenFolderDialogAsync();
        if (string.IsNullOrEmpty(directoryPath))
        {
            return;
        }

        var directoryViewModel = _directoryViewModelFactory.TryCreateViewModel(directoryPath);
        if (directoryViewModel == null)
        {
            await _dialogService.ShowMessage(
                $"{directoryPath}\r\nFolder not found.\r\nCheck the folder name and try again.",
                "Open Folder");
        }
        else
        {
            Directories.Add(directoryViewModel);
            SelectedDirectory = directoryViewModel;
        }
    }

    /// <summary>
    /// Opens a file and adds its directory ViewModel to the collection.
    /// </summary>
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

        var directoryViewModel = _directoryViewModelFactory.TryCreateViewModel(directoryPath, fileName);
        if (directoryViewModel == null)
        {
            await _dialogService.ShowMessage(
                $"{filePath}\r\nFile not found.\r\nCheck the file name and try again.",
                "Open Text File");
        }
        else
        {
            Directories.Add(directoryViewModel);
            SelectedDirectory = directoryViewModel;
        }
    }

    /// <summary>
    /// Restores the user settings to the ViewModel.
    /// </summary>
    private void RestoreSettings()
    {
        var directories = UserSettings.DirectoriesSettings
            .Select(_directoryViewModelFactory.TryCreateViewModel)
            .OfType<DirectoryViewModel>();

        Directories = new(directories);
        if (UserSettings.SelectedDirectoryIndex is { } index and >= 0 && index < Directories.Count)
        {
            SelectedDirectory = Directories[index];
        }
    }

    /// <summary>
    /// Saves the current user settings.
    /// </summary>
    [RelayCommand]
    public void SaveSettings()
    {
        UserSettings.DirectoriesSettings = Directories
            .Select(d => d.GetSettings())
            .ToList();

        UserSettings.SelectedDirectoryIndex = SelectedDirectory is not null 
            ? Directories.IndexOf(SelectedDirectory) 
            : null;

        if (!UserSettings.Equals(_savedUserSettings))
        {
            _userSettingsService.SaveSettings(UserSettings);
            _savedUserSettings = UserSettings.DeepCopy();
        }
    }

    /// <summary>
    /// Saves the user settings at a regular interval.
    /// </summary>
    private void SaveTimerElapsed(object? sender, ElapsedEventArgs e)
    {
        SaveSettings();
    }
}
