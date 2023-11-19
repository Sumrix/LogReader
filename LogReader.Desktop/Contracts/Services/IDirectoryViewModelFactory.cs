using LogReader.Desktop.ViewModels;

namespace LogReader.Desktop.Contracts.Services;

public interface IDirectoryViewModelFactory
{
    DirectoryViewModel CreateViewModel(string directoryPath);

    DirectoryViewModel CreateViewModel(string directoryPath, string fileName);
}