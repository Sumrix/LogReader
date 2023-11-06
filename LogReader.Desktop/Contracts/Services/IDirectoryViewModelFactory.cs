using LogReader.Desktop.ViewModels;

namespace LogReader.Desktop.Contracts.Services;

public interface IDirectoryViewModelFactory
{
    DirectoryViewModel? TryCreateViewModel(string directoryPath);
}