using LogReader.Core.Contracts.Services;
using LogReader.Core.Helpers;

namespace LogReader.Core.Models;

/// <summary>
/// Represents the data and state of a specific log file, including its content and metadata.
/// </summary>
public class FileData
{
    // Initial capacity set for large files to optimize memory allocation.
    private const int InitialCapacity = 500; 

    private readonly IFileReader _fileReader;
    private readonly IFileUpdateNotifier _fileUpdateNotifier;

    /// <summary>
    /// Information about the log file.
    /// </summary>
    public FileInfo FileInfo { get; }

    /// <summary>
    /// The position in the file where the last read operation stopped.
    /// Used to resume reading from this point.
    /// </summary>
    public long LastReadPosition { get; set; }

    /// <summary>
    /// Collection of <see cref="Record"/> parsed from the log file.
    /// </summary>
    public AppendOnlyObservableCollection<Record> Records { get; }

    /// <summary>
    /// Initializes a new instance of the FileData class for a given log file.
    /// </summary>
    /// <param name="fileInfo">The file information of the log file.</param>
    /// <param name="fileReader">The file reader to read the log file.</param>
    /// <param name="fileUpdateNotifier">The notifier to monitor file changes.</param>
    public FileData(FileInfo fileInfo, IFileReader fileReader, IFileUpdateNotifier fileUpdateNotifier)
    {
        FileInfo = fileInfo;
        _fileReader = fileReader;
        _fileUpdateNotifier = fileUpdateNotifier;
        Records = new(InitialCapacity);
    }

    /// <summary>
    /// Updates the file data by reading new content from the log file.
    /// </summary>
    public void Update()
    {
        _fileReader.Update(this);
    }

    /// <summary>
    /// Starts automatic updating of the file data.
    /// </summary>
    public void StartAutoUpdating()
    {
        _fileUpdateNotifier.UpdateRequired += FileUpdateNotifierOnUpdateRequired;
        _fileUpdateNotifier.Activate(FileInfo);
    }

    /// <summary>
    /// Handles the <c>UpdateRequired</c> event from the file update notifier.
    /// Triggers an update of the file data.
    /// </summary>
    private void FileUpdateNotifierOnUpdateRequired(object? sender, EventArgs e)
    {
        Update();
    }

    /// <summary>
    /// Stops the automatic updating of the file data.
    /// </summary>
    public void StopAutoUpdating()
    {
        _fileUpdateNotifier.Deactivate(FileInfo);
        _fileUpdateNotifier.UpdateRequired -= FileUpdateNotifierOnUpdateRequired;
    }
}