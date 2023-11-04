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
public partial class LogFileService : ILogFileService
{
    private readonly Regex _logRecordBeginningPattern = MyRegex();

    private const int BufferSize = 65536;
    
    /// <inheritdoc/>
    public async Task<LogFileModel?> TryReadAsync(string fileName)
    {
        return await Task.Run(() => TryRead(fileName));
    }

    /// <inheritdoc/>
    public LogFileModel? TryRead(string fileName)
    {
        if (!File.Exists(fileName))
        {
            return null;
        }

        var stringPool = new StringPool();
        List<LogRecordModel> recordModels = new();
        StringBuilder cumulativeLogRecord = new();
        const int maxHeaderSize = 100;
        var header = "";
        var data = DateTimeOffset.Now;

        using var fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read, BufferSize);
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
