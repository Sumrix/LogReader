using System;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using LogReader.Core.Models;

namespace LogReader.Desktop.ViewModels;

public partial class FileViewModel : ObservableObject
{
    [ObservableProperty]
    private FileModel _file;

    [ObservableProperty]
    private RecordModel _selectedRecord;

    public FileViewModel(FileModel file)
    {
        _file = file;
        _selectedRecord = file.Records.FirstOrDefault() ?? new("", DateTime.Now, "");
    }
    
    // For xaml previewer
    public FileViewModel()
    {
        var records = Enumerable.Range(0, 10)
            .Select(i => new RecordModel($"Log #{i}", DateTime.Now, $"Details #{i}"))
            .ToList();
        _file = new(records);
        _selectedRecord = _file.Records[0];
    }

    public static FileViewModel Empty { get; } = new(new(Array.Empty<RecordModel>()));
}