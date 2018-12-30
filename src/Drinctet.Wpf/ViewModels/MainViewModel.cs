using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Drinctet.Core;
using Drinctet.ViewModels;
using Newtonsoft.Json;
using Prism.Commands;
using Prism.Mvvm;

namespace Drinctet.Wpf.ViewModels
{
    public class MainViewModel : BindableBase
    {
        private readonly SemaphoreSlim _saveLock = new SemaphoreSlim(1, 1);
        private bool _isGameStarted;
        private bool _isSettingsOpen;
        private DelegateCommand _openSettingsCommand;
        private SettingsViewModel _settingsViewModel;
        private DrinctetStatus _status;
        private object _viewModel;

        public MainViewModel()
        {
            var welcomeViewModel = new WelcomeViewModel();
            welcomeViewModel.StartGame += OnStartGame;
            ViewModel = welcomeViewModel;
        }

        public object ViewModel
        {
            get => _viewModel;
            set => SetProperty(ref _viewModel, value);
        }

        public SettingsViewModel SettingsViewModel
        {
            get => _settingsViewModel;
            set => SetProperty(ref _settingsViewModel, value);
        }

        public bool IsSettingsOpen
        {
            get => _isSettingsOpen;
            set
            {
                if (SetProperty(ref _isSettingsOpen, value))
                {
                    _status.UpdatePlayers();
                    if (!value)
                        SaveSession();
                }
            }
        }

        public bool IsGameStarted
        {
            get => _isGameStarted;
            set => SetProperty(ref _isGameStarted, value);
        }

        public DelegateCommand OpenSettingsCommand
        {
            get
            {
                return _openSettingsCommand ?? (_openSettingsCommand = new DelegateCommand(() =>
                {
                    var vm = SettingsViewModel;
                    SettingsViewModel = null;
                    SettingsViewModel = vm;

                    IsSettingsOpen = true;
                }, () => IsGameStarted).ObservesProperty(() => IsGameStarted));
            }
        }

        private void OnStartGame(object sender, DrinctetStatus e)
        {
            e.UpdatePlayers();

            var gameViewModel = new GameViewModel(e);
            gameViewModel.PropertyChanged += GameViewModelOnPropertyChanged;

            ViewModel = gameViewModel;

            SettingsViewModel = new SettingsViewModel(e);
            IsGameStarted = true;

            _status = e;

            SaveSession();
        }

        private void GameViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(GameViewModel.CurrentSlide))
                SaveSession();
        }

        private async Task SaveSession()
        {
            if (_saveLock.Wait(0))
                try
                {
                    await Task.Run(() => File.WriteAllText("lastSession.json", JsonConvert.SerializeObject(_status)));
                }
                finally
                {
                    _saveLock.Release();
                }
        }
    }
}