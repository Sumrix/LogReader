using LogReader.Desktop.Models;
using LogReader.Desktop.ViewModels;

namespace LogReader.Desktop.Contracts.Services;

public interface IDirectoryViewModelFactory
{
    DirectoryViewModel? TryCreateViewModel(string directoryPath);

    DirectoryViewModel? TryCreateViewModel(string directoryPath, string fileName);

    DirectoryViewModel? TryCreateViewModel(DirectoryViewModelSettings directorySettings);
}