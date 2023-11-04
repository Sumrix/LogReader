namespace LogReader.Core.Models;

public record LogRecordModel(string Header, DateTimeOffset Data, string Details);