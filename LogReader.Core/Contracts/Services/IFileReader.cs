using LogReader.Core.Models;

namespace LogReader.Core.Contracts.Services;

/// <summary>
/// Provides functionality for reading log files and updating <see cref="FileData"/> with new records.
/// </summary>
public interface IFileReader
{
    /// <summary>
    /// Loads the data from the specified file into <see cref="FileData"/>.
    /// </summary>
    /// <param name="fileInfo">Information about the file to be read.</param>
    /// <returns>A <see cref="FileData"/> instance containing the loaded data.</returns>
    FileData Load(FileInfo fileInfo);

    /// <summary>
    /// Updates the given <see cref="FileData"/> instance with any new data from the associated file.
    /// </summary>
    /// <param name="fileData">The <see cref="FileData"/> instance to be updated.</param>
    void Update(FileData fileData);
}