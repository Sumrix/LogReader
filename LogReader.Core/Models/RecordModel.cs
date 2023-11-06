using System.Globalization;

namespace LogReader.Core.Models;

/// <summary>
/// Represents a single record in a file.
/// </summary>
/// <param name="Header">The header or title of the record.</param>
/// <param name="Date">The date and time when the record was created.</param>
/// <param name="Details">The detailed content of the record.</param>
public record RecordModel
(
    string Header,
    DateTimeOffset Date,
    string Details
)
{
    public string FullDetails => string.Format(CultureInfo.InvariantCulture, "{0:yyyy-MM-dd HH:mm:ss.fff zzz} {1}", Date, Details);
};