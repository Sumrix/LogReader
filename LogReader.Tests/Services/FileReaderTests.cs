using LogReader.Core.Contracts.Services;
using LogReader.Core.Models;
using LogReader.Core.Services;
using Moq;
using NUnit.Framework.Internal;

namespace LogReader.Tests.Services;

[TestFixture]
public class FileReaderTests
{
#pragma warning disable CS8618
    private IFileReader _fileReader;
    private IFileUpdateNotifier _fileMonitorFactory;
    private Mock<ILogParser> _mockLogParser;
    private FileInfo _existingFileInfo;
    private FileInfo _nonexistentFileInfo;
    private Randomizer _randomizer;

    private string _fileContent;
    private string? _capturedContent;
    private List<Record> _expectedRecords;
#pragma warning restore CS8618

    [SetUp]
    public void SetUp()
    {
        _randomizer = Randomizer.CreateRandomizer(); 
        _mockLogParser = new();
        _fileMonitorFactory = Mock.Of<IFileUpdateNotifier>();
        _fileReader = new FileReader(_mockLogParser.Object, Mock.Of<IFileUpdateNotifierFactory>());

        _nonexistentFileInfo = new("nonexistent.log");

        var existingFilePath = Path.GetTempFileName();
        _existingFileInfo = new(existingFilePath);
        _fileContent = _randomizer.GetString();
        File.WriteAllText(_existingFileInfo.FullName, _fileContent);
        
        _expectedRecords = new() { new(DateTimeOffset.Now, _randomizer.GetString()) };
        _mockLogParser.Setup(p => p.Parse(It.IsAny<Stream>()))
            .Callback<Stream>(stream =>
            {
                var reader = new StreamReader(stream);
                _capturedContent = reader.ReadToEnd();
            })
            .Returns(_expectedRecords);
    }

    [TearDown]
    public void TearDown()
    {
        if (File.Exists(_existingFileInfo.FullName))
        {
            File.Delete(_existingFileInfo.FullName);
        }
    }

    [Test]
    public void Load_WhenFileDoesNotExist_ShouldThrowFileNotFoundException()
    {
        // Act & Assert
        Assert.Throws<FileNotFoundException>(() => _fileReader.Load(_nonexistentFileInfo));
    }

    [Test]
    public void Load_WithFileContents_ShouldPassDataToLogParser()
    {
        // Arrange
        _capturedContent = null;

        // Act
        _fileReader.Load(_existingFileInfo);

        // Assert
        Assert.That(_capturedContent, Is.EqualTo(_fileContent));
    }

    [Test]
    public void Load_WithFileContents_ShouldUpdateLastReadPosition()
    {
        // Arrange
        _capturedContent = null;

        // Act
        var fileData = _fileReader.Load(_existingFileInfo);

        // Assert
        Assert.That(fileData.LastReadPosition, Is.EqualTo(_fileContent.Length));
    }

    [Test]
    public void Load_WhenLogParserProvidesRecords_ShouldReturnTheseRecords()
    {
        // Act
        var result = _fileReader.Load(_existingFileInfo);

        // Assert
        Assert.That(result.Records, Is.EqualTo(_expectedRecords));
    }

    [Test]
    public void Update_WhenFileDoesNotExist_ShouldThrowFileNotFoundException()
    {
        // Arrange
        var fileData = new FileData(_nonexistentFileInfo, _fileReader, _fileMonitorFactory);

        // Act & Assert
        Assert.Throws<FileNotFoundException>(() => _fileReader.Update(fileData));
    }

    [Test]
    public void Update_WithFileContents_ShouldPassDataToLogParser()
    {
        // Arrange
        var fileData = new FileData(_existingFileInfo, _fileReader, _fileMonitorFactory);
        _capturedContent = null;

        // Act
        _fileReader.Update(fileData);

        // Assert
        Assert.That(_capturedContent, Is.EqualTo(_fileContent));
    }
    
    [Test]
    public void Update_WhenNewData_ShouldUpdateLastReadPosition()
    {
        // Arrange
        var fileData = new FileData(_existingFileInfo, _fileReader, _fileMonitorFactory);
        var initialReadPosition = fileData.LastReadPosition;

        // Act
        _fileReader.Update(fileData);
        var updatedReadPosition = fileData.LastReadPosition;

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(initialReadPosition, Is.EqualTo(0));
            Assert.That(updatedReadPosition, Is.EqualTo(_fileContent.Length));
        });
    }

    [Test]
    public void Update_WhenNoNewData_ShouldNotUpdateLastReadPosition()
    {
        // Arrange
        var fileData = _fileReader.Load(_existingFileInfo);
        var initialRecordsCount = fileData.Records.Count;

        // Act
        _fileReader.Update(fileData);

        // Assert
        Assert.That(fileData.Records, Has.Count.EqualTo(initialRecordsCount));
    }

    [Test]
    public void Update_WhenLogParserProvidesRecords_ShouldAppendRecordsToEndOfRecordsCollection()
    {
        // Arrange
        var initialRecords = new List<Record> { new(DateTimeOffset.Now, _randomizer.GetString()) };
        var fileData = new FileData(_existingFileInfo, _fileReader, _fileMonitorFactory);
        fileData.Records.AddRange(initialRecords);
        
        // Act
        _fileReader.Update(fileData);

        // Assert
        Assert.That(fileData.Records, Is.EqualTo(initialRecords.Union(_expectedRecords)));
    }
}