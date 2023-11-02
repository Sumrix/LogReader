using LogReader.Console;

namespace LogReader.Tests.MSTest;

[TestClass]
public class ProgramTests
{
    [TestMethod]
    public async Task Main_FileExists_ShowsContent()
    {
        //Arrange
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

        var separatorLine = new string('-', 80);
        var pressAnyKeyMessage = "Press any key to display the next log record, or Ctrl+C to exit.";
        var expectedOutput = string.Join(
            Environment.NewLine + separatorLine + Environment.NewLine + pressAnyKeyMessage,
            logRecords.Skip(1)
        );
        expectedOutput += $"{Environment.NewLine}{separatorLine}{Environment.NewLine}No more log records to display.{Environment.NewLine}";

        var fileContent = string.Join(Environment.NewLine, logRecords);
        await File.WriteAllTextAsync(tempFileName, fileContent);

        var stringWriter = new StringWriter();
        System.Console.SetOut(stringWriter);
        var input = string.Join("", Enumerable.Repeat(Environment.NewLine, logRecords.Length - 1));
        System.Console.SetIn(new StringReader(input));
        
        //Act
        await Program.Main(new[] { tempFileName });

        //Assert
        var output = stringWriter.ToString();
        Assert.AreEqual(expectedOutput, output);

        // Cleanup
        File.Delete(tempFileName);
    }

    [TestMethod]
    public async Task Main_FileDoesNotExist_ShowsError()
    {
        //Arrange
        var nonExistentFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

        var stringWriter = new StringWriter();
        System.Console.SetOut(stringWriter);

        //Act
        await Program.Main(new[] { nonExistentFile });

        //Assert
        var output = stringWriter.ToString();
        Assert.AreEqual(
            $"Error: File \"{nonExistentFile}\" does not exist or cannot be accessed. Please check the file path and try again."
            + Environment.NewLine, output);
    }

    [TestMethod]
    public async Task Main_NoParams_ShowsError()
    {
        //Arrange
        var stringWriter = new StringWriter();
        System.Console.SetOut(stringWriter);

        //Act
        await Program.Main(Array.Empty<string>());

        //Assert
        var output = stringWriter.ToString();
        Assert.AreEqual($"Error: The file name cannot be empty. Please enter a file name." + Environment.NewLine, output);
    }
}