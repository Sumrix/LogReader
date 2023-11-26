using System.Collections.Generic;
using System.Linq;
using LogReader.Desktop.Helpers;
#pragma warning disable CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()

namespace LogReader.Desktop.Models;

public class UserSettings
{
    public double WindowWidth { get; set; } = 800;
    public double WindowHeight { get; set; } = 450;
    public int? WindowLeft { get; set; }
    public int? WindowTop { get; set; }
    public bool IsMaximized { get; set; }
    public List<DirectoryViewModelSettings> DirectoriesSettings { get; set; } = new();
    public int? SelectedDirectoryIndex { get; set; }

    public UserSettings DeepCopy()
    {
        var other = (UserSettings)MemberwiseClone();
        other.DirectoriesSettings = DirectoriesSettings
            .Select(d => d.DeepCopy())
            .ToList();
        return other;
    }

    public override bool Equals(object? obj)
    {
        return obj is UserSettings settings &&
               WindowWidth.ApproximatelyEquals(settings.WindowWidth, 0.001) &&
               WindowHeight.ApproximatelyEquals(settings.WindowHeight, 0.001) &&
               WindowLeft == settings.WindowLeft &&
               WindowTop == settings.WindowTop &&
               IsMaximized == settings.IsMaximized &&
               Enumerable.SequenceEqual(DirectoriesSettings, settings.DirectoriesSettings) &&
               SelectedDirectoryIndex == settings.SelectedDirectoryIndex;
    }
}
