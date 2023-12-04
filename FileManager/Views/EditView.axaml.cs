using Avalonia.Controls;
using FileManager.ViewModels;

namespace FileManager.Views
{
    public partial class EditView : UserControl
    {
        public EditView()
        {
            InitializeComponent();
            DataContext = new EditViewModel();
        }
    }
}
