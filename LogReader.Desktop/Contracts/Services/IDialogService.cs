using System.Threading.Tasks;

namespace LogReader.Desktop.Contracts.Services;

public interface IDialogService
{
    /// <summary>
    /// Opens a file dialog and returns the selected file path.
    /// </summary>
    /// <param name="filter">File dialog filter (e.g., "Text files (*.txt)|*.txt|All files (*.*)|*.*").</param>
    /// <returns>The path of the selected file, or null if the dialog was canceled.</returns>
    Task<string?> OpenFileDialogAsync(string filter);

    /// <summary>
    /// Opens a folder dialog and returns the selected folder path.
    /// </summary>
    /// <returns>The path of the selected folder, or null if the dialog was canceled.</returns>
    Task<string?> OpenFolderDialogAsync();

    /// <summary>
    /// Displays a message box with the specified text.
    /// </summary>
    /// <param name="message">The message to display.</param>
    /// <param name="title">The title of the message box.</param>
    Task ShowMessage(string message, string title);
}
