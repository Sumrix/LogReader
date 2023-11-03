using LogReader.Core.Models;

namespace LogReader.Core.Contracts.Services;

/// <summary>
/// Provides methods to handle log file operations.
/// </summary>
public interface ILogFileService
{
    /// <summary>
    /// Attempts to read the content of the specified log file.
    /// </summary>
    /// <param name="fileName">The name or path of the file to read.</param>
    /// <returns>The data of the log file if the file was read successfully; otherwise, null.</returns>
    Task<LogFileModel?> TryReadAsync(string fileName);
}