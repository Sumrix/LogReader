using LogReader.Core.Models;

namespace LogReader.Core.Contracts.Services;

/// <summary>
/// Provides methods to handle file operations.
/// </summary>
public interface IFileService
{
    /// <summary>
    /// Asynchronously attempts to read the content of the specified file.
    /// </summary>
    /// <param name="filePath">The path of the file to read.</param>
    /// <returns>A <see cref="FileModel"/> containing the file's content if the file was read successfully; otherwise, null.</returns>
    Task<FileModel?> TryReadAsync(string filePath);
}