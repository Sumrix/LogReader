namespace LogReader.Desktop.Models;

public record DirectoryViewModelSettings
(
    string Path,
    FileViewModelSettings? SelectedFile
)
{
    public DirectoryViewModelSettings DeepCopy()
    {
        return new(Path, SelectedFile?.DeepCopy());
    }
}