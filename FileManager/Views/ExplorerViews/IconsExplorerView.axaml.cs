using Avalonia.Controls;
using FileManager.ViewModels.ExplorerViewModels;

namespace FileManager.Views.ExplorerViews
{
    public partial class IconsExplorerView : UserControl
    {
        public IconsExplorerView()
        {
            InitializeComponent();
            DataContext = new IconsExplorerViewModel();
        }
    }
}
