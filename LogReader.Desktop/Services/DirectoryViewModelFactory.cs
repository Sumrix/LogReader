using System.IO;
using LogReader.Core.Contracts.Services;
using LogReader.Desktop.Contracts.Services;
using LogReader.Desktop.Models;
using LogReader.Desktop.ViewModels;

namespace LogReader.Desktop.Services;

internal class DirectoryViewModelFactory : IDirectoryViewModelFactory
{
    private readonly IFileReader _fileReader;

    public DirectoryViewModelFactory(IFileReader fileReader)
    {
        _fileReader = fileReader;
    }

    public DirectoryViewModel? TryCreateViewModel(string directoryPath)
    {
        var directoryInfo = new DirectoryInfo(directoryPath);
        if (!directoryInfo.Exists)
        {
            return null;
        }

        return new(directoryInfo, _fileReader);
    }

    public DirectoryViewModel? TryCreateViewModel(string directoryPath, string fileName)
    {
        var directoryInfo = new DirectoryInfo(directoryPath);
        if (!directoryInfo.Exists)
        {
            return null;
        }

        var directoryViewModel = new DirectoryViewModel(directoryInfo, fileName, _fileReader);

        return directoryViewModel;
    }

    public DirectoryViewModel? TryCreateViewModel(DirectoryViewModelSettings directorySettings)
    {
        var directoryInfo = new DirectoryInfo(directorySettings.Path);
        if (!directoryInfo.Exists)
        {
            return null;
        }

        var directoryViewModel = directorySettings.SelectedFile is not null
            ? new DirectoryViewModel(directoryInfo, directorySettings.SelectedFile, _fileReader)
            : new(directoryInfo, _fileReader);

        return directoryViewModel;
    }
}