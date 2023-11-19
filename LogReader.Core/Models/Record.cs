namespace LogReader.Core.Models;

/// <summary>
/// Represents a single log entry with a timestamp and associated message.
/// </summary>
/// <param name="Timestamp">The time at which the log record was created. Represents both the date and time of the log entry.</param>
/// <param name="Message">The content or message of the log entry. Contains the detailed information or data of the log record.</param>
public record Record
(
    DateTimeOffset Timestamp,
    string Message
);
