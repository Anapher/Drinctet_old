using System;
using System.IO;
using Drinctet.Core;
using Newtonsoft.Json;
using Prism.Commands;

namespace Drinctet.Wpf.ViewModels
{
    public class WelcomeViewModel
    {
        private DelegateCommand _openLastSessionCommand;

        private DelegateCommand _startNewGameCommand;

        public WelcomeViewModel()
        {
            SettingsViewModel = new SettingsViewModel(new DrinctetStatus());
        }

        public DelegateCommand OpenLastSessionCommand
        {
            get
            {
                return _openLastSessionCommand ?? (_openLastSessionCommand =
                           new DelegateCommand(() =>
                           {
                               var raw = File.ReadAllText("lastSession.json");
                               var status = JsonConvert.DeserializeObject<DrinctetStatus>(raw);
                               StartGame?.Invoke(this, status);
                           }, () => File.Exists("lastSession.json")));
            }
        }

        public DelegateCommand StartNewGameCommand
        {
            get
            {
                return _startNewGameCommand ?? (_startNewGameCommand = new DelegateCommand(() =>
                {
                    StartGame?.Invoke(this, SettingsViewModel.Status);
                }));
            }
        }

        public SettingsViewModel SettingsViewModel { get; }

        public event EventHandler<DrinctetStatus> StartGame;
    }
}