using System.Globalization;
using System.Text;
using CommunityToolkit.HighPerformance.Buffers;
using LogReader.Core.Contracts.Services;
using LogReader.Core.Models;

namespace LogReader.Core.Services;

/// <summary>
/// Parses log entries from a stream into a collection of Record objects.
/// </summary>
public class LogParser : ILogParser
{
    private readonly StringPool _stringPool = new(); // For memory optimization
    private const int DateLength = 30; // Length of "0001-01-01 00:00:00.000 +00:00"

    public IEnumerable<Record> Parse(Stream logStream)
    {
        if (logStream == null)
        {
            throw new ArgumentNullException(nameof(logStream));
        }

        var reader = new StreamReader(logStream);
        var currentMessage = new StringBuilder();
        var currentTimestamp = DateTimeOffset.MinValue;

        while (reader.ReadLine() is { } currentLine)
        {
            if (currentLine.Length >= DateLength && TryParseDateTimeOffset(currentLine[..DateLength], out var timestamp))
            {
                if (currentTimestamp != default)
                {
                    yield return new(currentTimestamp, _stringPool.GetOrAdd(currentMessage.ToString().Trim()));
                    currentMessage.Clear();
                }

                currentTimestamp = timestamp;
                currentMessage.Append(currentLine[DateLength..]);
            }
            else if (currentTimestamp != default)
            {
                currentMessage.AppendLine().Append(currentLine);
            }
        }

        if (currentTimestamp != default)
        {
            yield return new(currentTimestamp, _stringPool.GetOrAdd(currentMessage.ToString().Trim()));
        }
    }

    private static bool TryParseDateTimeOffset(string dateTimeString, out DateTimeOffset dateTimeOffset)
    {
        return DateTimeOffset.TryParseExact(dateTimeString, "yyyy-MM-dd HH:mm:ss.fff zzz", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTimeOffset);
    }
}