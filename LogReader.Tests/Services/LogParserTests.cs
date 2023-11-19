using System.Globalization;
using LogReader.Core.Contracts.Services;
using LogReader.Core.Models;
using LogReader.Core.Services;
using NUnit.Framework.Internal;

namespace LogReader.Tests.Services;

[TestFixture]
public class LogParserTests
{
#pragma warning disable CS8618
    private ILogParser _logParser;
    private Randomizer _randomizer;
#pragma warning restore CS8618

    [SetUp]
    public void SetUp()
    {
        _logParser = new LogParser();
        _randomizer = Randomizer.CreateRandomizer();
    }

    [Test]
    public void Parse_WithSingleLineRecord_ShouldReturnCorrectRecord()
    {
        // Arrange
        var (expectedRecord, logContent) = GenerateLogMessage();
        using var stream = GenerateStreamFromString(logContent);

        // Act
        var result = _logParser.Parse(stream).ToList();

        // Assert
        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result[0], Is.EqualTo(expectedRecord));
    }

    [Test]
    public void Parse_WithMultiLineRecord_ShouldReturnCorrectRecord()
    {
        // Arrange
        var (expectedRecord, logContent) = GenerateLogMessage(multiline: true);
        using var stream = GenerateStreamFromString(logContent);

        // Act
        var result = _logParser.Parse(stream).ToList();

        // Assert
        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result[0], Is.EqualTo(expectedRecord));
    }

    [Test]
    public void Parse_WithMultipleRecords_ShouldReturnAllRecords()
    {
        // Arrange
        var (expectedRecords, logContent) = GenerateLogMessages();
        using var stream = GenerateStreamFromString(logContent);

        // Act
        var results = _logParser.Parse(stream).ToList();

        // Assert
        CollectionAssert.AreEqual(expectedRecords, results);
    }

    [Test]
    public void Parse_WithEmptyStream_ShouldReturnEmptyEnumerable()
    {
        // Arrange
        using var stream = GenerateStreamFromString("");

        // Act
        var results = _logParser.Parse(stream).ToList();

        // Assert
        Assert.That(results, Is.Empty);
    }

    [Test]
    public void Parse_WithMalformedDate_ShouldSkipInvalidRecord()
    {
        // Arrange
        var (expectedRecord, logContent) = GenerateLogMessage();
        logContent = _randomizer.GetString() + Environment.NewLine + logContent;
        using var stream = GenerateStreamFromString(logContent);

        // Act
        var results = _logParser.Parse(stream).ToList();

        // Assert
        Assert.That(results, Has.Count.EqualTo(1));
        Assert.That(results[0], Is.EqualTo(expectedRecord));
    }

    [Test]
    public void Parse_WhenStreamIsNull_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
        {
            _ =_logParser.Parse(null!).ToList();
        });
    }

    [Test]
    public void Parse_WithRecordHavingDateOnly_ShouldReturnRecordWithEmptyMessage()
    {
        // Arrange
        var (expectedRecord, logContent) = GenerateLogMessage();
        logContent = logContent[..^(expectedRecord.Message.Length + 1)];
        expectedRecord = expectedRecord with { Message = "" };
        using var stream = GenerateStreamFromString(logContent);

        // Act
        var results = _logParser.Parse(stream).ToList();

        // Assert
        Assert.That(results, Has.Count.EqualTo(1));
        Assert.That(results[0], Is.EqualTo(expectedRecord));
    }

    private static Stream GenerateStreamFromString(string s)
    {
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        writer.Write(s);
        writer.Flush();
        stream.Position = 0;
        return stream;
    }

    private (Record record, string logContent) GenerateLogMessage(bool multiline = false)
    {
        // Generate the date ignoring micro and nano seconds.
        var ticks = _randomizer.NextInt64(DateTimeOffset.MinValue.Ticks / 10000, DateTimeOffset.MaxValue.Ticks / 10000) * 10000;
        var timestamp = new DateTimeOffset(ticks, TimeSpan.Zero);
        var message = multiline
            ? string.Join(Environment.NewLine, Enumerable.Range(1, _randomizer.Next(3, 10)).Select(_ => _randomizer.GetString(_randomizer.Next(10, 30))))
            : _randomizer.GetString(_randomizer.Next(10, 30));
        var record = new Record(timestamp, message);
        var logContent = string.Format(CultureInfo.InvariantCulture, "{0:yyyy-MM-dd HH:mm:ss.fff zzz} {1}", timestamp, message);
        return (record, logContent);
    }

    private (List<Record> expectedRecords, string logContent) GenerateLogMessages()
    {
        var items = Enumerable.Range(1, _randomizer.Next(3, 10))
            .Select(_ => GenerateLogMessage(_randomizer.NextBool()))
            .ToList();
        var expectedRecords = items
            .Select(i => i.record)
            .ToList();
        var logContent = string.Join(Environment.NewLine, items.Select(i => i.logContent));
        return (expectedRecords, logContent);
    }
}
