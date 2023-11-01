using System.Text;
using System.Text.RegularExpressions;
using LogReader.Core.Contracts.Services;
using LogReader.Core.Models;

namespace LogReader.Core.Services;

/// <summary>
/// Represents a service that provides file operations for log files.
/// </summary>
public partial class LogFileService : ILogFileService
{
    private readonly Regex _logRecordBeginningPattern = MyRegex();

    /// <inheritdoc/>
    public async Task<LogFileModel?> TryReadAsync(string fileName)
    {
        if (!File.Exists(fileName))
        {
            return null;
        }

        List<LogRecordModel> recordModels = new();
        StringBuilder cumulativeLogRecord = new();

        await foreach (var line in File.ReadLinesAsync(fileName))
        {
            if (_logRecordBeginningPattern.IsMatch(line))
            {
                AppendCurrentRecord();
                cumulativeLogRecord.Clear();
                cumulativeLogRecord.Append(line);
            }
            else
            {
                if (cumulativeLogRecord.Length > 0)
                {
                    cumulativeLogRecord.AppendLine().Append(line);
                }
            }
        }
    
        AppendCurrentRecord();

        return new(recordModels);

        void AppendCurrentRecord()
        {
            if (cumulativeLogRecord.Length > 0)
            {
                recordModels.Add(new(cumulativeLogRecord.ToString()));
            }
        }
    }

    [GeneratedRegex("^\\d\\d\\d\\d-\\d\\d-\\d\\d")]
    private static partial Regex MyRegex();
}
