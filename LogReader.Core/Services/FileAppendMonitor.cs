using LogReader.Core.Contracts.Services;

namespace LogReader.Core.Services;

/// <summary>
/// Monitors a specific file for changes, particularly focusing on file append operations.
/// When changes are detected, it triggers an UpdateRequired event to signal that the file
/// has been updated and needs to be re-read.
/// </summary>
public sealed class FileAppendMonitor : IFileUpdateNotifier, IDisposable
{
    public event EventHandler? UpdateRequired;
    private FileSystemWatcher? _fileWatcher;
    private long _lastFileSize;
    private readonly object _syncObject = new();

    /// <summary>
    /// Activates monitoring on the specified file. Sets up a file system watcher
    /// to observe changes in file size or last write time.
    /// </summary>
    /// <param name="fileInfo">FileInfo object representing the file to monitor.</param>
    public void Activate(FileInfo fileInfo)
    {
        if (fileInfo == null)
        {
            throw new ArgumentNullException(nameof(fileInfo));
        }

        if (fileInfo.DirectoryName == null)
        {
            throw new ArgumentException($"The field {nameof(fileInfo.DirectoryName)} cannot be null");
        }

        if (_fileWatcher?.EnableRaisingEvents == true)
        {
            return;
        }

        _lastFileSize = fileInfo.Length;
        _fileWatcher = new()
        {
            Path = fileInfo.DirectoryName,
            Filter = fileInfo.Name,
            NotifyFilter = NotifyFilters.Size
        };

        _fileWatcher.Changed += OnChanged;
        _fileWatcher.EnableRaisingEvents = true;
    }

    /// <summary>
    /// Deactivates monitoring on the current file, if any, and cleans up resources.
    /// </summary>
    /// <param name="fileInfo">FileInfo object representing the file to stop monitoring.</param>
    public void Deactivate(FileInfo fileInfo)
    {
        if (fileInfo == null)
        {
            throw new ArgumentNullException(nameof(fileInfo));
        }

        if (_fileWatcher is { EnableRaisingEvents: true })
        {
            _fileWatcher.Changed -= OnChanged;
            _fileWatcher.EnableRaisingEvents = false;
            _fileWatcher.Dispose();
        }
    }

    /// <summary>
    /// Event handler for the FileSystemWatcher's Changed event. Checks if the file size
    /// has increased and triggers the UpdateRequired event if new data has been appended.
    /// </summary>
    private void OnChanged(object sender, FileSystemEventArgs e)
    {
        lock (_syncObject)
        {
            var newFileSize = new FileInfo(e.FullPath).Length;
            if (newFileSize <= _lastFileSize)
            {
                return;
            }

            _lastFileSize = newFileSize;
            UpdateRequired?.Invoke(this, EventArgs.Empty);
        }
    }

    /// <summary>
    /// Disposes the FileSystemWatcher and any other resources.
    /// </summary>
    public void Dispose()
    {
        _fileWatcher?.Dispose();
    }
}