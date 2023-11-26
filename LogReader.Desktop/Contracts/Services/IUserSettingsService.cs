using LogReader.Desktop.Models;

namespace LogReader.Desktop.Contracts.Services;

/// <summary>
/// Defines a service for loading and saving user settings.
/// </summary>
public interface IUserSettingsService
{
    /// <summary>
    /// Loads the user settings.
    /// </summary>
    /// <returns>Loaded <see cref="UserSettings"/>.</returns>
    UserSettings LoadSettings();

    /// <summary>
    /// Saves the user settings.
    /// </summary>
    /// <param name="settings">The user settings to save.</param>
    void SaveSettings(UserSettings settings);
}