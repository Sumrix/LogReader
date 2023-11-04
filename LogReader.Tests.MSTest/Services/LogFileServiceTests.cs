using System.Globalization;
using LogReader.Core.Contracts.Services;
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
    [DataRow("")]
    [DataRow("""
            Lorem ipsum dolor sit amet, consectetur adipiscing elit,
            sed do eiusmod tempor incididunt ut labore et dolore magna
            aliqua. Ut enim ad minim veniam, quis nostrud exercitation
            ullamco laboris nisi ut aliquip ex ea commodo consequat.
            Duis aute irure dolor in reprehenderit in voluptate velit
            esse cillum dolore eu fugiat nulla pariatur. Excepteur sint
            occaecat cupidatat non proident, sunt in culpa qui officia
            deserunt mollit anim id est laborum.
            """)]
    public async Task TryRead_FileExistsNoRecords_ReturnsFileModelAndNoRecords(string fileContent)
    {
        // Arrange
        var tempFileName = Path.GetTempFileName();
        await File.WriteAllTextAsync(tempFileName, fileContent);

        // Act
        var logFile = await _logFileService.TryReadAsync(tempFileName);

        // Assert
        Assert.IsNotNull(logFile);

        var expectedRecords = Array.Empty<string>();
        var actualRecords = logFile.Records.Select(r => r.Details).ToList();
        CollectionAssert.AreEqual(expectedRecords, actualRecords);

        // Cleanup
        File.Delete(tempFileName);
    }

    [TestMethod]
    public async Task TryRead_FileExistsWithRecords_ReturnsFileModelAndRecords()
    {
        // Arrange
        const string inputFileName = @".\Assets\logs_input.txt";
        const string outputFileName = @".\Assets\expected_output_records.txt";
        var expectedOutput = await File.ReadAllTextAsync(outputFileName);
        var expectedRecords = expectedOutput.Split(Environment.NewLine + "---" + Environment.NewLine);

        // Act
        var logFile = await _logFileService.TryReadAsync(inputFileName);

        // Assert
        Assert.IsNotNull(logFile);
        
        var actualRecords = logFile.Records
            .Select(r => string.Format(CultureInfo.InvariantCulture, "{0:yyyy-MM-dd HH:mm:ss.fff zzz} {1}", r.Data, r.Details))
            .ToList();
        CollectionAssert.AreEqual(expectedRecords, actualRecords);
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