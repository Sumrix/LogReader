using LogReader.Core.Contracts.Services;
using LogReader.Core.Models;

namespace LogReader.Core.Services;

public class DirectoryService : IDirectoryService
{
    public DirectoryModel? TryLoad(string directoryPath) =>
        Directory.Exists(directoryPath)
            ? new(directoryPath, Directory.GetFiles(directoryPath))
            : null;
}
