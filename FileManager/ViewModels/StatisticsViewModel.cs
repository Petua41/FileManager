using FileManager.Models;
using FileManager.Services;
using ReactiveUI;
using System.Collections.ObjectModel;

namespace FileManager.ViewModels
{
    internal class StatisticsViewModel : ViewModelBase
    {
        public StatisticsViewModel()
        {
            CurrentDirectory.CurrentDirectorySwitched += (sender, e) => this.RaisePropertyChanged(nameof(Values));
        }

        public static ObservableCollection<FileStatRecord> Values
        {
            get
            {
                StatisticsCalculator calc = new(CurrentDirectory.CurrentDir);
                return [new FileStatRecord("Total", calc.CountTotal().ToString()),
                    new FileStatRecord("Files", calc.CountFiles().ToString()),
                    new FileStatRecord("Direcories", calc.CountDirectories().ToString()),
                    new FileStatRecord("Hidden", calc.CountHiddenTotal().ToString()),
                    new FileStatRecord("Hidden files", calc.CountHiddenFiles().ToString()),
                    new FileStatRecord("Hidden directories", calc.CountHiddenDirectories().ToString())];
            }
        }
    }
}