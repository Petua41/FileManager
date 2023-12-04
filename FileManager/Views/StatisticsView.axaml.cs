using Avalonia.Controls;
using FileManager.ViewModels;

namespace FileManager.Views
{
    public partial class StatisticsView : UserControl
    {
        public StatisticsView()
        {
            InitializeComponent();
            DataContext = new StatisticsViewModel();
        }
    }
}
