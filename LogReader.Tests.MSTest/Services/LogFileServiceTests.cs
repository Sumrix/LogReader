using LogReader.Core.Contracts.Services;

namespace LogReader.Core.Services.Tests;

[TestClass]
public class LogFileServiceTests
{
    private readonly ILogFileService _logFileService;

    public LogFileServiceTests()
    {
        _logFileService = new LogFileService();
    }

    [TestMethod]
    public void TryRead_FileExists_ReturnsTrueAndSetsContent()
    {
        // Arrange
        var tempFileName = Path.GetTempFileName();
        File.WriteAllText(tempFileName, "Test content");

        // Act
        var result = _logFileService.TryRead(tempFileName, out var content);

        // Assert
        Assert.IsTrue(result);
        Assert.AreEqual("Test content", content);

        // Cleanup
        File.Delete(tempFileName);
    }

    [TestMethod]
    public void TryRead_FileDoesNotExist_ReturnsFalse()
    {
        // Arrange
        var nonExistentFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

        // Act
        var result = _logFileService.TryRead(nonExistentFile, out var content);

        // Assert
        Assert.IsFalse(result);
        Assert.IsNull(content);
    }
}