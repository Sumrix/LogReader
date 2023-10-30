using Avalonia.Controls;
using LogReader.Desktop.ViewModels;

namespace LogReader.Desktop.Views;

public partial class ShellWindow : Window
{
    public ShellWindow(ShellViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }

    public ShellWindow()
    {
        InitializeComponent();
    }
}
