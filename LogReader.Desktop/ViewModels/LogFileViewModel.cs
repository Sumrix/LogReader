using System;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using LogReader.Core.Models;

namespace LogReader.Desktop.ViewModels;

public partial class LogFileViewModel : ObservableObject
{
    [ObservableProperty]
    private LogFileModel _logFile;

    [ObservableProperty]
    private LogRecordModel _selectedLogRecord;

    public LogFileViewModel(LogFileModel logFile)
    {
        _logFile = logFile;
        _selectedLogRecord = logFile.Records.FirstOrDefault() ?? new("", DateTime.Now, "");
    }

    // For designer
    public LogFileViewModel()
    {
        var records = Enumerable.Range(0, 10)
            .Select(i => new LogRecordModel($"Log #{i}", DateTime.Now, $"Details #{i}"))
            .ToList();
        _logFile = new(records);
        _selectedLogRecord = _logFile.Records[0];
    }
}