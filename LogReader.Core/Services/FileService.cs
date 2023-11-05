using System.Text;
using System.Text.RegularExpressions;
using CommunityToolkit.HighPerformance.Buffers;
using LogReader.Core.Contracts.Services;
using LogReader.Core.Helpers;
using LogReader.Core.Models;

namespace LogReader.Core.Services;

/// <summary>
/// Represents a service that provides file operations for log files.
/// </summary>
public partial class FileService : IFileService
{
    private const int BufferSize = 65536; // For file load speed optimization
    private readonly Regex _logRecordBeginningPattern = MyRegex();

    /// <inheritdoc />
    public async Task<FileModel?> TryReadAsync(string filePath) => await Task.Run(() => TryRead(filePath));
    
    private FileModel? TryRead(string filePath)
    {
        if (!File.Exists(filePath))
        {
            return null;
        }

        List<RecordModel> recordModels = new();
        StringBuilder cumulativeLogRecord = new();
        const int maxHeaderSize = 100;
        var header = "";
        var data = DateTimeOffset.Now;
        var stringPool = new StringPool(); // For memory optimization

        using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, BufferSize);
        using var streamReader = new StreamReader(fileStream);

        while (streamReader.ReadLine() is { } line)
        {
            if (_logRecordBeginningPattern.IsMatch(line.AsSpan()))
            {
                AppendCurrentRecord();
                cumulativeLogRecord.Clear();
                header = line.TruncateRight(maxHeaderSize, true);
                data = DateTimeOffset.Parse(header[..30]);
                cumulativeLogRecord.Append(line.AsSpan(31));
            }
            else
            {
                if (cumulativeLogRecord.Length > 0)
                {
                    cumulativeLogRecord.AppendLine().Append(line.AsSpan());
                }
            }
        }

        AppendCurrentRecord();

        return new(recordModels);

        void AppendCurrentRecord()
        {
            if (cumulativeLogRecord.Length > 0)
            {
                recordModels.Add(new(header, data, stringPool.GetOrAdd(cumulativeLogRecord.ToString())));
            }
        }
    }
    
    [GeneratedRegex("^\\d\\d\\d\\d-\\d\\d-\\d\\d")]
    private static partial Regex MyRegex();
}