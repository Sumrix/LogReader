namespace LogReader.Core.Contracts.Services;

/// <summary>
/// Provides functionality for monitoring file changes and notifying when updates are required.
/// </summary>
public interface IFileUpdateNotifier
{
    /// <summary>
    /// Occurs when an update is required due to changes in the file.
    /// </summary>
    event EventHandler UpdateRequired;

    /// <summary>
    /// Activates the notifier for the specified file.
    /// </summary>
    /// <param name="fileInfo">The file to monitor for changes.</param>
    void Activate(FileInfo fileInfo);

    /// <summary>
    /// Deactivates the notifier, stopping monitoring of the file.
    /// </summary>
    /// <param name="fileInfo">The file to stop monitoring.</param>
    void Deactivate(FileInfo fileInfo);
}