using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LogReader.Core.Models;
using LogReader.Core.Services;
using LogReader.Desktop.Models;

namespace LogReader.Desktop.ViewModels;

/// <summary>
/// ViewModel for representing and interacting with a file and its records.
/// </summary>
public partial class FileViewModel : ObservableObject
{
    [ObservableProperty]
    private string? _details;

    [ObservableProperty]
    private FileData _file;

    [ObservableProperty]
    private List<Record> _selectedRecords = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="FileViewModel" /> class with the specified file data.
    /// </summary>
    /// <param name="file">File data to be represented by this ViewModel.</param>
    public FileViewModel(FileData file)
    {
        File = file ?? throw new ArgumentNullException(nameof(file));
    }

    public FileViewModel(FileData file, IEnumerable<int> selectedRecordIndices) : this(file)
    {
        SelectedRecords = selectedRecordIndices
            .Where(i => i > 0 && i < File.Records.Count)
            .Select(i => File.Records[i])
            .ToList();
    }

    /// <summary>
    /// Constructor for XAML previewer with sample data.
    /// </summary>
    public FileViewModel()
    {
        var fileReader = new FileReader(new LogParser(), new FileAppendMonitorFactory());
        var fileInfo = new FileInfo(@".\LogReader.Desktop\Assets\example_log_file.txt");
        File = fileReader.Load(fileInfo);
    }

    public FileViewModelSettings GetSettings()
    {
        return new(File.FileInfo.Name, SelectedRecords.Select(File.Records.IndexOf).ToList());
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

    partial void OnSelectedRecordsChanged(List<Record> value)
    {
        var messages = value
            .Select(r => r.Message);

        Details = string.Join(Environment.NewLine + Environment.NewLine, messages);
    }

    [RelayCommand]
    private void SelectionsChanged(IList selectedItems)
    {
        SelectedRecords = selectedItems
            .Cast<Record>()
            .ToList();

        var messages = SelectedRecords
            .Select(r => r.Message);

        Details = string.Join(Environment.NewLine + Environment.NewLine, messages);
    }
}