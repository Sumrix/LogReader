using LogReader.Core.Models;

namespace LogReader.Core.Contracts.Services;

/// <summary>
/// Provides methods to handle directory operations.
/// </summary>
public interface IDirectoryService
{
    /// <summary>
    /// Asynchronously attempts to load the list of file names in the specified directory.
    /// </summary>
    /// <param name="directoryPath">The path of the directory whose file names are to be loaded.</param>
    /// <returns>A <see cref="DirectoryModel"/> containing the file names if the directory was loaded successfully; otherwise, null.</returns>
    DirectoryModel? TryLoad(string directoryPath);
}