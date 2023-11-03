using LogReader.Console;

namespace LogReader.Tests.MSTest;

using System;

[TestClass]
public class ProgramTests
{
    [TestMethod]
    public async Task Main_FileExists_ShowsContent()
    {
        //Arrange
        const string inputFileName = @".\Assets\logs_input.txt";
        const string outputFileName = @".\Assets\expected_output_console.txt";
        var expectedOutput = await File.ReadAllTextAsync(outputFileName);
        var outputStream = new StringWriter();
        var inputStream = new StringReader(string.Join("", Enumerable.Repeat(Environment.NewLine, 5)));
        
        Console.SetOut(outputStream);
        Console.SetIn(inputStream);
        
        //Act
        await Program.Main(new[] { inputFileName });

        //Assert
        var output = outputStream.ToString();
        Assert.AreEqual(expectedOutput, output);
    }

    [TestMethod]
    public async Task Main_FileDoesNotExist_ShowsError()
    {
        //Arrange
        var nonExistentFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

        var stringWriter = new StringWriter();
        Console.SetOut(stringWriter);

        //Act
        await Program.Main(new[] { nonExistentFile });

        //Assert
        var output = stringWriter.ToString();
        Assert.AreEqual($"Error: File \"{nonExistentFile}\" does not exist or cannot be accessed. " +
                        $"Please check the file path and try again." + Environment.NewLine, output);
    }

    [TestMethod]
    public async Task Main_NoParams_ShowsError()
    {
        //Arrange
        var stringWriter = new StringWriter();
        Console.SetOut(stringWriter);

        //Act
        await Program.Main(Array.Empty<string>());

        //Assert
        var output = stringWriter.ToString();
        Assert.AreEqual("Error: The file name cannot be empty. Please enter a file name." + Environment.NewLine, output);
    }
}