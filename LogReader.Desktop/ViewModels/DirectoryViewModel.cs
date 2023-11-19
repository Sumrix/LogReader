using System.Collections.Generic;
using System.IO;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LogReader.Core.Contracts.Services;
using LogReader.Core.Services;

namespace LogReader.Desktop.ViewModels;

/// <summary>
/// ViewModel for representing and interacting with a directory and its files.
/// </summary>
public partial class DirectoryViewModel : ObservableObject
{
    private readonly IFileReader _fileReader;

    /// <summary>
    /// The full path of the directory.
    /// </summary>
    public string Path { get; }

    /// <summary>
    /// List of files in the directory.
    /// </summary>
    public IReadOnlyList<FileInfo> Files { get; }

    [ObservableProperty]
    private FileViewModel? _selectedFile;

    [ObservableProperty]
    private FileInfo? _selectedFileInfo;

    /// <summary>
    /// Initializes a new instance of the <see cref="DirectoryViewModel"/> class for the specified directory.
    /// </summary>
    /// <param name="directoryInfo">Directory information to represent.</param>
    /// <param name="fileReader">File reader service for loading file data.</param>
    public DirectoryViewModel(DirectoryInfo directoryInfo, IFileReader fileReader)
    {
        _fileReader = fileReader;
        
        Path = directoryInfo.FullName;
        Files = directoryInfo.GetFiles();
    }
    
    /// <summary>
    /// Constructor for XAML previewer with sample data.
    /// </summary>
    public DirectoryViewModel()
    {
        _fileReader = new FileReader(new LogParser(), new FileAppendMonitorFactory());
        var directoryInfo = new DirectoryInfo(@".\LogReader.Desktop\Assets\");

        Path = directoryInfo.FullName;
        Files = directoryInfo.GetFiles();

        SelectedFileInfo = Files.SingleOrDefault(f => f.Name == "random_log_file.txt");
    }

    [RelayCommand]
    public void ReloadFile()
    {
        if (SelectedFileInfo is null)
        {
            SelectedFile = null;
            return;
        }

        var logFile = _fileReader.Load(SelectedFileInfo);
        SelectedFile = new(logFile);
    }

    /// <summary>
    /// Called when this ViewModel becomes the active (focused) ViewModel.
    /// </summary>
    public void OnActivated()
    {
        SelectedFile?.OnActivated();
    }

    /// <summary>
    /// Called when this ViewModel is no longer the active (focused) ViewModel.
    /// </summary>
    public void OnDeactivated()
    {
        SelectedFile?.OnDeactivated();
    }

    // ReSharper disable once UnusedParameterInPartialMethod
    partial void OnSelectedFileInfoChanged(FileInfo? value)
    {
        ReloadFile();
    }

    partial void OnSelectedFileChanged(FileViewModel? oldValue, FileViewModel? newValue)
    {
        oldValue?.OnDeactivated();
        newValue?.OnActivated();
    }
}