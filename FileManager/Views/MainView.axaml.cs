using Avalonia.Controls;
using FileManager.ViewModels;

namespace FileManager.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
        DataContext = new MainViewModel();
    }
}
