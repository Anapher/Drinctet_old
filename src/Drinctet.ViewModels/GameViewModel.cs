using System.Globalization;
using Drinctet.Core;
using Drinctet.ViewModels.Manager;
using Drinctet.ViewModels.Resources;
using Drinctet.ViewModels.ViewModelBase;

namespace Drinctet.ViewModels
{
    public class GameViewModel : PropertyChangedBase
    {
        private readonly ICardsProvider _cardsProvider;
        private readonly ScreenGameManager _screenGameManager;
        private readonly DrinctetStatus _status;
        private ISlideViewModel _currentSlide;
        private RelayCommand _goBackCommand;
        private ISlideViewModel _lastSlide;

        private RelayCommand _nextSlideCommand;

        public GameViewModel(DrinctetStatus status)
        {
            AppResources.Culture = CultureInfo.GetCultureInfo("de");

            _status = status;
            _screenGameManager = new ScreenGameManager(_status, new ResourceTextTranslation());
            _cardsProvider = DependencyService.Get<ICardsProvider>();
            NextSlideCommand.Execute(null);
        }

        public GameViewModel()
        {
        }

        public ISlideViewModel CurrentSlide
        {
            get => _currentSlide;
            set => SetProperty(value, ref _currentSlide);
        }

        public RelayCommand NextSlideCommand
        {
            get
            {
                return _nextSlideCommand ?? (_nextSlideCommand = new RelayCommand(parameter =>
                {
                    _lastSlide = _currentSlide;
                    CurrentSlide = _screenGameManager.Next(_cardsProvider);
                }));
            }
        }

        public RelayCommand GoBackCommand
        {
            get
            {
                return _goBackCommand ??
                       (_goBackCommand = new RelayCommand(parameter => { CurrentSlide = _lastSlide; }));
            }
        }
    }
}