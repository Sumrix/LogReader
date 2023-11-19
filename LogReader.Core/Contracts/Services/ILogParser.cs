using LogReader.Core.Models;

namespace LogReader.Core.Contracts.Services;

/// <summary>
/// Parses log entries from a stream into a collection of <see cref="Record"/> objects.
/// </summary>
public interface ILogParser
{
    /// <summary>
    /// Parses the provided stream into a collection of log records.
    /// </summary>
    /// <param name="logStream">The stream to parse.</param>
    /// <returns>An enumerable collection of parsed <see cref="Record"/> objects.</returns>
    IEnumerable<Record> Parse(Stream logStream);
}