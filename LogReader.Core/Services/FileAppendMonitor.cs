using System.Timers;
using LogReader.Core.Contracts.Services;
using Timer = System.Timers.Timer;

namespace LogReader.Core.Services;

/// <summary>
/// Monitors a specific file for changes, particularly focusing on file append operations.
/// </summary>
/// <remarks>
/// When changes are detected, it triggers an <see cref="UpdateRequired" /> event to signal that the file
/// has been updated and needs to be re-read.
/// </remarks>
public sealed class FileAppendMonitor : IFileUpdateNotifier, IDisposable
{
    private readonly object _activationSyncRoot = new();
    private readonly Timer _delayedUpdateTimer;
    private readonly FileSystemWatcher _fileWatcher;

    private bool _isActive;
    private FileInfo? _monitoredFile;

    public event EventHandler? UpdateRequired;

    public FileAppendMonitor()
    {
        _fileWatcher = new()
        {
            NotifyFilter = NotifyFilters.Size | NotifyFilters.LastWrite
        };
        _fileWatcher.Changed += OnFileChanged;

        _delayedUpdateTimer = new(TimeSpan.FromSeconds(5));
        _delayedUpdateTimer.Elapsed += OnTimerElapsed;
    }

    /// <summary>
    /// Disposes resources.
    /// </summary>
    public void Dispose()
    {
        _fileWatcher.Dispose();
        _delayedUpdateTimer.Dispose();
    }

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

        // Using lock to maintain data integrity during parallel Activate and Deactivate calls
        lock (_activationSyncRoot)
        {
            if (_isActive)
            {
                if (_monitoredFile == fileInfo)
                {
                    // Already active on this file
                    return;
                }

                Deactivate(); // Deactivate the current file before activating a new one.
            }

            // So that we are not affected by external changes to fileInfo, we want our own copy of the object
            _monitoredFile = new(fileInfo.FullName);
            // Ensure we have the original file size
            _monitoredFile.Refresh();

            _fileWatcher.Path = _monitoredFile.DirectoryName ?? "";
            _fileWatcher.Filter = _monitoredFile.Name;
            _fileWatcher.EnableRaisingEvents = true;

            _delayedUpdateTimer.Start();

            _isActive = true;
        }
    }

    /// <summary>
    /// Deactivates monitoring on the current file, if any.
    /// </summary>
    public void Deactivate()
    {
        if (_monitoredFile == null)
        {
            return;
        }

        // Using lock to maintain data integrity during parallel Activate and Deactivate calls
        lock (_activationSyncRoot)
        {
            if (!_isActive)
            {
                return;
            }

            _fileWatcher.EnableRaisingEvents = false;
            _delayedUpdateTimer.Stop();

            _isActive = false;
        }
    }

    private void OnFileChanged(object sender, FileSystemEventArgs e)
    {
        RequestUpdateIfFileIncreased();
    }

    private void OnTimerElapsed(object? o, ElapsedEventArgs elapsedEventArgs)
    {
        RequestUpdateIfFileIncreased();
    }

    private void RequestUpdateIfFileIncreased()
    {
        if (_monitoredFile == null)
        {
            return;
        }

        var lastFileSize = _monitoredFile.Length;

        // Temporarily disable the file watcher to prevent triggering OnFileChanged.
        _fileWatcher.EnableRaisingEvents = false;
        _monitoredFile.Refresh();
        _fileWatcher.EnableRaisingEvents = true;

        var newFileSize = _monitoredFile.Length;

        if (newFileSize <= lastFileSize)
        {
            return;
        }

        // Restart the timer to ensure that OnTimerElapsed is triggered only if there's
        // a significant delay since the last RequestUpdate call.
        _delayedUpdateTimer.Stop();
        _delayedUpdateTimer.Start();

        RequestUpdate();
    }

    private void RequestUpdate()
    {
        UpdateRequired?.Invoke(this, EventArgs.Empty);
    }
}