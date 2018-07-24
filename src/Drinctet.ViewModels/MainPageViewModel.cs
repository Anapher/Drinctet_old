using System.Collections.ObjectModel;
using Drinctet.ViewModels.ViewModelBase;

namespace Drinctet.ViewModels
{
    public class MainPageViewModel : PropertyChangedBase, IViewModel
    {
        private RelayCommand _addPlayerCommand;
        private RelayCommand _startCommand;

        public MainPageViewModel()
        {
            Players = new ObservableCollection<PlayerViewModel>();
            Players.Add(new PlayerViewModel {Name = "Vincent"});
            Players.Add(new PlayerViewModel {Name = "Bursod"});
            Players.Add(new PlayerViewModel {Name = "Nora", IsFemale = true});
        }

        public ObservableCollection<PlayerViewModel> Players { get; }

        public RelayCommand AddPlayerCommand
        {
            get
            {
                return _addPlayerCommand ?? (_addPlayerCommand = new RelayCommand(parameter =>
                {
                    Players.Add(new PlayerViewModel());
                }));
            }
        }

        public RelayCommand StartCommand
        {
            get
            {
                return _startCommand ?? (_startCommand = new RelayCommand(parameter =>
                {
                    ViewInterface.Show(new GameViewModel());
                }));
            }
        }

        public IViewInterface ViewInterface { get; set; }
    }
}