using System.IO;
using System.IO.IsolatedStorage;
using System.Text.Json;
using LogReader.Desktop.Contracts.Services;
using LogReader.Desktop.Models;

namespace LogReader.Desktop.Services;

public class UserSettingsService : IUserSettingsService
{
    private const string FileName = "UserSettings.json";

    private static IsolatedStorageFile GetIsolatedStorageFile()
    {
        return IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null);
    }

    public  UserSettings LoadSettings()
    {
        var storageFile = GetIsolatedStorageFile();
        if (!storageFile.FileExists(FileName))
        {
            return new();
        }

        try
        {
            using var storageFileStream = new IsolatedStorageFileStream(FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, storageFile);
            var settingsJson = new StreamReader(storageFileStream).ReadToEnd();
            return JsonSerializer.Deserialize<UserSettings>(settingsJson) ?? new UserSettings();
        }
        catch
        {
            return new();
        }
    }

    public void SaveSettings(UserSettings settings)
    {
        try
        {
            var storageFile = GetIsolatedStorageFile();
            using var storageFileStream = new IsolatedStorageFileStream(FileName, FileMode.Create,
                FileAccess.Write, FileShare.ReadWrite, storageFile);
            using var writer = new StreamWriter(storageFileStream);
            var settingsJson = JsonSerializer.Serialize(settings);
            writer.Write(settingsJson);
        }
        catch
        {
            // ignored
        }
    }
}