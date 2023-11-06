using LogReader.Core.Contracts.Services;
using LogReader.Core.Models;
using LogReader.Core.Services;

namespace LogReader.Tests.MSTest.Services;

[TestClass]
public class LogFileServiceTests
{
    private readonly IFileService _fileService;

    public LogFileServiceTests()
    {
        _fileService = new FileService();
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
    public async Task TryReadAsync_FileWithoutRecords_ReturnsEmptyFile(string fileContent)
    {
        // Arrange
        var tempFileName = Path.GetTempFileName();
        await File.WriteAllTextAsync(tempFileName, fileContent);

        // Act
        FileModel? logFile;
        try
        {
            logFile = await _fileService.TryReadAsync(tempFileName);
        }
        finally
        {
            File.Delete(tempFileName);
        }

        // Assert
        Assert.IsNotNull(logFile);
        Assert.AreEqual(0, logFile.Records.Count);
    }

    [TestMethod]
    public async Task TryReadAsync_FileWithRecords_ReturnsFileWithRecords()
    {
        // Arrange
        const string inputFileName = @".\Assets\logs_input.txt";
        const string outputFileName = @".\Assets\expected_output_records.txt";
        var expectedOutput = await File.ReadAllTextAsync(outputFileName);
        var expectedRecords = expectedOutput.Split(Environment.NewLine + "---" + Environment.NewLine);

        // Act
        var logFile = await _fileService.TryReadAsync(inputFileName);

        // Assert
        Assert.IsNotNull(logFile);
        
        var actualRecords = logFile.Records.Select(r => r.FullDetails).ToList();
        CollectionAssert.AreEqual(expectedRecords, actualRecords);
    }

    [TestMethod]
    public async Task TryReadAsync_FileDoesNotExist_ReturnsNull()
    {
        // Arrange
        var nonExistentFile = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

        // Act
        var logFile = await _fileService.TryReadAsync(nonExistentFile);

        // Assert
        Assert.IsNull(logFile);
    }
}