namespace LogReader.Core.Models;

/// <summary>
/// Represents a directory and its files.
/// </summary>
/// <param name="Path">The full path of the directory.</param>
/// <param name="FileNames">A read-only list of file names within the directory.</param>
public record DirectoryModel
(
    string Path,
    IReadOnlyList<string> FileNames
);