using Avalonia.Controls;
using FileManager.ViewModels.ExplorerViewModels;

namespace FileManager.Views.ExplorerViews
{
    public partial class TreeExplorerView : UserControl
    {
        public TreeExplorerView()
        {
            InitializeComponent();
            DataContext = new TreeExplorerViewModel();
        }
    }
}
