using LogReader.Console;

namespace LogReader.Tests.MSTest;

[TestClass]
public class ProgramTests
{
    [TestMethod]
    public void Main_FileExists_ShowsContent()
    {
        //Arrange
        var tempFileName = Path.GetTempFileName();
        const string fileContent = "Test content";
        File.WriteAllText(tempFileName, fileContent);

        var stringWriter = new StringWriter();
        System.Console.SetOut(stringWriter);

        //Act
        Program.Main(new[] { tempFileName });

        //Assert
        var output = stringWriter.ToString();
        Assert.AreEqual(fileContent + Environment.NewLine, output);

        // Cleanup
        File.Delete(tempFileName);
    }

    [TestMethod]
    public void Main_FileDoesNotExist_ShowsError()
    {
        //Arrange
        var nonExistentFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

        var stringWriter = new StringWriter();
        System.Console.SetOut(stringWriter);

        //Act
        Program.Main(new[] { nonExistentFile });

        //Assert
        var output = stringWriter.ToString();
        Assert.AreEqual(
            $"Error: File \"{nonExistentFile}\" does not exist or cannot be accessed. Please check the file path and try again."
            + Environment.NewLine, output);
    }

    [TestMethod]
    public void Main_NoParams_ShowsError()
    {
        //Arrange
        var stringWriter = new StringWriter();
        System.Console.SetOut(stringWriter);

        //Act
        Program.Main(Array.Empty<string>());

        //Assert
        var output = stringWriter.ToString();
        Assert.AreEqual($"Error: The file name cannot be empty. Please enter a file name." + Environment.NewLine, output);
    }
}