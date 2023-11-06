using LogReader.Core.Contracts.Services;
using LogReader.Core.Models;

namespace LogReader.Core.Services;

public class DirectoryService : IDirectoryService
{
    public DirectoryModel? TryLoad(string directoryPath)
    {
        if (!Directory.Exists(directoryPath))
        {
            return null;
        }

        var fileNames = Directory.GetFiles(directoryPath)
            .Select(Path.GetFileName)
            .ToList();
        return new(directoryPath, fileNames!);
    }
}
