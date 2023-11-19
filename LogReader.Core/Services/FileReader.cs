using LogReader.Core.Contracts.Services;
using LogReader.Core.Models;

namespace LogReader.Core.Services;

/// <summary>
/// Provides functionality for reading log files and updating FileData with new records.
/// </summary>
public class FileReader : IFileReader
{
    private const int BufferSize = 65536; // 64 KB buffer to speed up loading
    private readonly ILogParser _parser;
    private readonly IFileUpdateNotifierFactory _updateNotifierFactory;

    public FileReader(ILogParser parser, IFileUpdateNotifierFactory updateNotifierFactory)
    {
        _parser = parser ?? throw new ArgumentNullException(nameof(parser));
        _updateNotifierFactory = updateNotifierFactory ?? throw new ArgumentNullException(nameof(updateNotifierFactory));
    }

    public FileData Load(FileInfo fileInfo)
    {
        if (fileInfo == null)
        {
            throw new ArgumentNullException(nameof(fileInfo));
        }

        if (!fileInfo.Exists)
        {
            throw new FileNotFoundException("File not found", fileInfo.FullName);
        }

        var file = new FileData(fileInfo, this, _updateNotifierFactory.Create());
        Update(file);
        return file;
    }

    public void Update(FileData fileData)
    {
        if (fileData == null)
        {
            throw new ArgumentNullException(nameof(fileData));
        }

        fileData.FileInfo.Refresh();

        if (!fileData.FileInfo.Exists)
        {
            throw new FileNotFoundException("File not found", fileData.FileInfo.FullName);
        }

        var fileLength = fileData.FileInfo.Length;
        var lastReadPosition = fileData.LastReadPosition;

        if (fileLength == lastReadPosition)
        {
            return;
        }

        using var stream = fileData.FileInfo.Open(new FileStreamOptions
        {
            Mode = FileMode.Open,
            Access = FileAccess.Read,
            Share = FileShare.ReadWrite,
            BufferSize = BufferSize,
        });
        stream.Position = lastReadPosition;

        var newRecords = _parser.Parse(stream);
        fileData.Records.AddRange(newRecords);
        
        fileData.LastReadPosition = stream.Position;
    }
}