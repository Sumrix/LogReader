﻿using LogReader.Core.Contracts.Services;
using LogReader.Core.Services;

namespace LogReader.Tests.MSTest.Services;

[TestClass]
public class LogFileServiceTests
{
    private readonly ILogFileService _logFileService;

    public LogFileServiceTests()
    {
        _logFileService = new LogFileService();
    }

    [TestMethod]
    public async Task TryRead_FileExistsButEmpty_ReturnsFileModelAndNoRecords()
    {
        // Arrange
        var tempFileName = Path.GetTempFileName();
        const string fileContent = "";
        await File.WriteAllTextAsync(tempFileName, fileContent);

        // Act
        var logFile = await _logFileService.TryReadAsync(tempFileName);

        // Assert
        Assert.IsNotNull(logFile);

        var expectedRecords = Array.Empty<string>();
        var actualRecords = logFile.Records.Select(r => r.Text).ToList();
        CollectionAssert.AreEqual(expectedRecords, actualRecords);

        // Cleanup
        File.Delete(tempFileName);
    }

    [TestMethod]
    public async Task TryRead_FileExistsWithRandomText_ReturnsFileModelAndNoRecords()
    {
        // Arrange
        var tempFileName = Path.GetTempFileName();
        var fileContent =
            // ReSharper disable once StringLiteralTypo
            """
            Lorem ipsum dolor sit amet, consectetur adipiscing elit,
            sed do eiusmod tempor incididunt ut labore et dolore magna
            aliqua. Ut enim ad minim veniam, quis nostrud exercitation
            ullamco laboris nisi ut aliquip ex ea commodo consequat.
            Duis aute irure dolor in reprehenderit in voluptate velit
            esse cillum dolore eu fugiat nulla pariatur. Excepteur sint
            occaecat cupidatat non proident, sunt in culpa qui officia
            deserunt mollit anim id est laborum.
            """;
        await File.WriteAllTextAsync(tempFileName, fileContent);

        // Act
        var logFile = await _logFileService.TryReadAsync(tempFileName);

        // Assert
        Assert.IsNotNull(logFile);

        var expectedRecords = Array.Empty<string>();
        var actualRecords = logFile.Records.Select(r => r.Text).ToList();
        CollectionAssert.AreEqual(expectedRecords, actualRecords);

        // Cleanup
        File.Delete(tempFileName);
    }

    [TestMethod]
    public async Task TryRead_FileExistsWithRecords_ReturnsFileModelAndRecords()
    {
        // Arrange
        var tempFileName = Path.GetTempFileName();
        var logRecords = new[]
        {
            """
            confusing file start
            without records
            """,
            "2010-01-01 simple log record",
            "2010-01-05 2010-01-06 multiple dates",
            """
            2010-01-02 log record with empty lines and spaces at the end

               
            """,
            """
            2010-01-03 log
            record
            with
            multiple
            lines
            2010-01-OO false record
             2010-01-07 shifted date
            """,
            """
            2010-01-04 multiline log record
            with spaces and empty lines at the end

              
            """
        };
        var fileContent = string.Join(Environment.NewLine, logRecords);
        await File.WriteAllTextAsync(tempFileName, fileContent);

        // Act
        var logFile = await _logFileService.TryReadAsync(tempFileName);

        // Assert
        Assert.IsNotNull(logFile);

        var expectedRecords = logRecords[1..]; // Without the first false lines
        var actualRecords = logFile.Records.Select(r => r.Text).ToList();
        CollectionAssert.AreEqual(expectedRecords, actualRecords);

        // Cleanup
        File.Delete(tempFileName);
    }

    [TestMethod]
    public async Task TryRead_FileDoesNotExist_ReturnsNull()
    {
        // Arrange
        var nonExistentFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

        // Act
        var logFile = await _logFileService.TryReadAsync(nonExistentFile);

        // Assert
        Assert.IsNull(logFile);
    }
}