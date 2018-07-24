using Drinctet.Core;
using Drinctet.ViewModels.Manager;
using Drinctet.ViewModels.Resources;
using Drinctet.ViewModels.Slides;
using Drinctet.ViewModels.ViewModelBase;

namespace Drinctet.ViewModels
{
    public class GameViewModel : PropertyChangedBase
    {
        private readonly ICardsProvider _cardsProvider;
        private readonly ScreenGameManager _screenGameManager;
        private readonly DrinctetStatus _status;
        private ISlideViewModel _currentSlide;
        
        private RelayCommand _nextSlideCommand;

        public GameViewModel(DrinctetStatus status)
        {
            _status = status;
            _screenGameManager = new ScreenGameManager(_status, new ResourceTextTranslation());
            _cardsProvider = DependencyService.Get<ICardsProvider>();
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
                    CurrentSlide = _screenGameManager.Next(_cardsProvider);
                }));
            }
        }
    }
}