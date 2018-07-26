using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using Drinctet.Core;
using Drinctet.ViewModels.Resources;
using Drinctet.ViewModels.ViewModelBase;

namespace Drinctet.ViewModels
{
    public class MainPageViewModel : PropertyChangedBase, IViewModel
    {
        private RelayCommand _addPlayerCommand;
        private RelayCommand _startCommand;

        public MainPageViewModel()
        {
            AppResources.Culture = CultureInfo.GetCultureInfo("de");

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
                    var settings = new DrinctetStatus();

                    var counter = 1;
                    settings.Players = Players.Select(x =>
                        new PlayerInfo(counter++, x.IsFemale ? Gender.Female : Gender.Male) {Name = x.Name}).ToList();
                    settings.InitializePlayers();

                    ViewInterface.Show(new GameViewModel(settings));
                }));
            }
        }

        public IViewInterface ViewInterface { get; set; }
    }
}