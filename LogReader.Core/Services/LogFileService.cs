using LogReader.Core.Contracts.Services;

namespace LogReader.Core.Services;

/// <summary>
/// Represents a service that provides file operations for log files.
/// </summary>
public class LogFileService : ILogFileService
{
    /// <inheritdoc/>
    public bool TryRead(string fileName, out string? content)
    {
        throw new NotImplementedException();
    }
}
