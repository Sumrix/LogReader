using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LogReader.Core.Contracts.Services;
using LogReader.Core.Helpers;
using LogReader.Core.Models;

namespace LogReader.Desktop.ViewModels;

public partial class DirectoryViewModel : ObservableObject
{
    private const int HeaderLength = 15;

    private readonly IFileService _fileService;
    private readonly DirectoryModel _directoryModel;
    
    public string Path => _directoryModel.Path.TruncateLeft(HeaderLength, true);

    public IReadOnlyList<string> FileNames => _directoryModel.FileNames;

    [ObservableProperty]
    private FileViewModel _selectedFile;

    [ObservableProperty]
    private string? _selectedFileName;

    public DirectoryViewModel(DirectoryModel directoryModel, IFileService fileService)
    {
        _directoryModel = directoryModel;
        _fileService = fileService;
        _selectedFile = FileViewModel.Empty;

        PropertyChanged += async (_, e) =>
        {
            if (e.PropertyName == nameof(SelectedFileName))
            {
                await Refresh();
            }
        };
    }
    
    // For xaml previewer
    public DirectoryViewModel()
    {
        _fileService = null!;
        _directoryModel = new(
            @"..\..\..\..\LogReader.Tests.MSTest\Assets\",
            new []{"logs_input.txt", "expected_output_records.txt"});
        _selectedFile = FileViewModel.Empty;
    }

    [RelayCommand]
    public async Task Refresh()
    {
        if (SelectedFileName is null)
        {
            SelectedFile = FileViewModel.Empty;
            return;
        }

        var filePath = System.IO.Path.Combine(_directoryModel.Path, SelectedFileName);
        var logFile = await _fileService.TryReadAsync(filePath)
                      ?? throw new InvalidOperationException("File doesn't exist");
        SelectedFile = new(logFile);
    }
}