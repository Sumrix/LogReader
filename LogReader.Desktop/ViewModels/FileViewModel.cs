using System;
using System.Collections;
using System.IO;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LogReader.Core.Models;
using LogReader.Core.Services;

namespace LogReader.Desktop.ViewModels;

/// <summary>
/// ViewModel for representing and interacting with a file and its records.
/// </summary>
public partial class FileViewModel : ObservableObject
{
    [ObservableProperty]
    private FileData _file;

    [ObservableProperty]
    private string? _details;

    /// <summary>
    /// Initializes a new instance of the <see cref="FileViewModel"/> class with the specified file data.
    /// </summary>
    /// <param name="file">File data to be represented by this ViewModel.</param>
    public FileViewModel(FileData file)
    {
        _file = file ?? throw new ArgumentNullException(nameof(file));
    }
    
    /// <summary>
    /// Constructor for XAML previewer with sample data.
    /// </summary>
    public FileViewModel()
    {
        var fileReader = new FileReader(new LogParser(), new FileAppendMonitorFactory());
        var fileInfo = new FileInfo(@".\LogReader.Desktop\Assets\random_log_file.txt");
        File = fileReader.Load(fileInfo);
    }

    /// <summary>
    /// Called when this ViewModel becomes the active (focused) ViewModel.
    /// Handles the activation logic, such as starting auto-updating.
    /// </summary>
    public void OnActivated()
    {
        File.Update();
        File.StartAutoUpdating();
    }

    /// <summary>
    /// Called when this ViewModel is no longer the active (focused) ViewModel.
    /// Handles the deactivation logic, such as stopping auto-updating.
    /// </summary>
    public void OnDeactivated()
    {
        File.StopAutoUpdating();
    }

    [RelayCommand]
    private void SelectionsChanged(IList selectedRecords)
    {
        var messages = selectedRecords
            .Cast<Record>()
            .Select(r => r.Message);

        Details = string.Join(Environment.NewLine + Environment.NewLine, messages);
    }
}