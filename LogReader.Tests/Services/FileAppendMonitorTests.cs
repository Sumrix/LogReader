using LogReader.Core.Services;
using NUnit.Framework.Internal;

namespace LogReader.Tests.Services;

[TestFixture]
public class FileAppendMonitorTests
{
#pragma warning disable CS8618
    private FileInfo _fileInfo;
    private Randomizer _randomizer;
#pragma warning restore CS8618

    [SetUp]
    public void SetUp()
    {
        _randomizer = Randomizer.CreateRandomizer(); 
        _fileInfo = new(Path.GetTempFileName());
    }

    [TearDown]
    public void TearDown()
    {
        if (_fileInfo.Exists)
        {
            _fileInfo.Delete();
        }
    }

    [Test]
    public async Task Activate_WhenFileAppended_ShouldRaiseUpdateRequiredEvent()
    {
        // Arrange
        var updateRequiredRaised = new TaskCompletionSource<bool>();
        using var fileAppendMonitor = new FileAppendMonitor();
        fileAppendMonitor.UpdateRequired += (_, _) => updateRequiredRaised.SetResult(true);
        fileAppendMonitor.Activate(_fileInfo);

        // Act
        await File.AppendAllTextAsync(_fileInfo.FullName, _randomizer.GetString());

        // Assert
        Assert.That(await updateRequiredRaised.Task, Is.True);
    }

    [Test]
    public void Activate_AfterActivation_ShouldNotThrowException()
    {
        // Arrange
        using var fileAppendMonitor = new FileAppendMonitor();
        fileAppendMonitor.Activate(_fileInfo);

        // Act & Assert
        Assert.DoesNotThrow(() => fileAppendMonitor.Activate(_fileInfo));
    }

    [Test]
    public void Activate_WithNullFileData_ShouldThrowArgumentNullException()
    {
        // Arrange
        using var fileAppendMonitor = new FileAppendMonitor();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => fileAppendMonitor.Activate(null!));
    }

    [Test]
    public async Task Deactivate_AfterActivation_ShouldPreventUpdateRequiredEvent()
    {
        // Arrange
        var wasUpdateRequired = false;
        using var fileAppendMonitor = new FileAppendMonitor();
        fileAppendMonitor.UpdateRequired += (_, _) => wasUpdateRequired = true;
        fileAppendMonitor.Activate(_fileInfo);

        // Act
        fileAppendMonitor.Deactivate();
        await File.AppendAllTextAsync(_fileInfo.FullName, _randomizer.GetString());
        await Task.Delay(10);

        // Assert
        Assert.That(wasUpdateRequired, Is.False);
    }

    [Test]
    public void Deactivate_AfterDeactivation_ShouldNotThrowException()
    {
        // Arrange
        using var fileAppendMonitor = new FileAppendMonitor();
        fileAppendMonitor.Activate(_fileInfo);
        fileAppendMonitor.Deactivate();

        // Act & Assert
        Assert.DoesNotThrow(() => fileAppendMonitor.Deactivate());
    }
}
