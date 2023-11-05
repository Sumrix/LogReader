namespace LogReader.Core.Models;

/// <summary>
/// Represents the content of a file as a collection of records.
/// </summary>
/// <param name="Records">A read-only list of records contained in the file.</param>
public record FileModel
(
    IReadOnlyList<RecordModel> Records
);