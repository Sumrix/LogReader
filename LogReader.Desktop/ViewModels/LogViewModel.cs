using CommunityToolkit.Mvvm.ComponentModel;

namespace LogReader.Desktop.ViewModels;

public partial class LogViewModel : ObservableObject
{
    [ObservableProperty]
    private string _logs = "Log text";

    public LogViewModel(string logs)
    {
        _logs = logs;
    }

    // For designer
    public LogViewModel()
    {
    }
}