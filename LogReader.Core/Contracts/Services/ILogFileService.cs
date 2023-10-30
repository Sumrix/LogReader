using System.Diagnostics.CodeAnalysis;

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
    /// <param name="content">When this method returns, contains the content of the file if the file was read successfully, or null if the file could not be accessed.</param>
    /// <returns><c>true</c> if the file was read successfully; otherwise, <c>false</c>.</returns>
    bool TryRead(string fileName, [NotNullWhen(true)] out string? content);
}