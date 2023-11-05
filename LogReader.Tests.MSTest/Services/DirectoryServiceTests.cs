using LogReader.Core.Contracts.Services;
using LogReader.Core.Models;
using LogReader.Core.Services;

namespace LogReader.Tests.MSTest.Services;

[TestClass]
public class DirectoryServiceTests
{
    private readonly IDirectoryService _directoryService;

    public DirectoryServiceTests()
    {
        _directoryService = new DirectoryService();
    }

    [TestMethod]
    public void TryLoadAsync_DirectoryWithFiles_ReturnsDirectoryWithFiles()
    {
        // Arrange
        var notEmptyDirectoryPath = Directory.CreateTempSubdirectory().FullName;
        var expectedFilesCount = Random.Shared.Next(1, 10);
        var expectedFiles = new List<string>();
        for (var i = 0; i < expectedFilesCount; i++)
        {
            var fileName = Path.Combine(notEmptyDirectoryPath, Guid.NewGuid().ToString());
            expectedFiles.Add(fileName);
            File.Create(fileName).Close();
        }

        // Act
        DirectoryModel? directory;
        try
        {
            directory = _directoryService.TryLoad(notEmptyDirectoryPath);
        }
        finally
        {
            Directory.Delete(notEmptyDirectoryPath, true);
        }

        // Assert
        Assert.IsNotNull(directory);
        Assert.AreEqual(notEmptyDirectoryPath, directory.Path);
        CollectionAssert.AreEquivalent(expectedFiles, directory.FileNames.ToList());
    }


    [TestMethod]
    public void TryLoadAsync_EmptyDirectory_ReturnsEmptyDirectory()
    {
        // Arrange
        var emptyDirectoryPath = Directory.CreateTempSubdirectory().FullName;

        // Act
        DirectoryModel? directory;
        try
        {
            directory = _directoryService.TryLoad(emptyDirectoryPath);
        }
        finally
        {
            Directory.Delete(emptyDirectoryPath, true);
        }

        // Assert
        Assert.IsNotNull(directory);
        Assert.AreEqual(emptyDirectoryPath, directory.Path);
        Assert.AreEqual(0, directory.FileNames.Count);
    }

    [TestMethod]
    public void TryLoadAsync_DirectoryDoesNotExist_ReturnsNull()
    {
        // Arrange
        var nonExistentPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

        // Act
        var directory = _directoryService.TryLoad(nonExistentPath);

        // Assert
        Assert.IsNull(directory);
    }
}