using Avalonia.Controls;
using FileManager.ViewModels;

namespace FileManager.Views
{
    public partial class ExplorerView : UserControl
    {
        public ExplorerView()
        {
            InitializeComponent();
            DataContext = new ExplorerViewModel();      // for non-static ViewModels it`s the only way
        }
    }
}
