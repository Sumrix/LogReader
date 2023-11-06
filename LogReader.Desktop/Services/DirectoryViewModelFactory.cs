using LogReader.Core.Contracts.Services;
using LogReader.Desktop.Contracts.Services;
using LogReader.Desktop.ViewModels;

namespace LogReader.Desktop.Services;

internal class DirectoryViewModelFactory : IDirectoryViewModelFactory
{
    private readonly IFileService _fileService;
    private readonly IDirectoryService _directoryService;

    public DirectoryViewModelFactory(IFileService fileService, IDirectoryService directoryService)
    {
        _fileService = fileService;
        _directoryService = directoryService;
    }

    public DirectoryViewModel? TryCreateViewModel(string directoryPath) =>
        _directoryService.TryLoad(directoryPath) is { } directoryModel
            ? new(directoryModel, _fileService)
            : null;
}