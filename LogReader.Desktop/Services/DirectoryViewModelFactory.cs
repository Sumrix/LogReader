using System.IO;
using System.Linq;
using LogReader.Core.Contracts.Services;
using LogReader.Desktop.Contracts.Services;
using LogReader.Desktop.ViewModels;

namespace LogReader.Desktop.Services;

internal class DirectoryViewModelFactory : IDirectoryViewModelFactory
{
    private readonly IFileReader _fileReader;

    public DirectoryViewModelFactory(IFileReader fileReader)
    {
        _fileReader = fileReader;
    }

    public DirectoryViewModel CreateViewModel(string directoryPath)
    {
        var directoryInfo = new DirectoryInfo(directoryPath);
        return new(directoryInfo, _fileReader);
    }

    public DirectoryViewModel CreateViewModel(string directoryPath, string fileName)
    {
        var directoryInfo = new DirectoryInfo(directoryPath);
        var directoryViewModel = new DirectoryViewModel(directoryInfo, _fileReader);

        directoryViewModel.SelectedFileInfo = directoryViewModel.Files.First(f => f.Name == fileName);

        return directoryViewModel;
    }
}