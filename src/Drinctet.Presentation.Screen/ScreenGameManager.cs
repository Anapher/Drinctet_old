using System;
using Drinctet.Core;
using Drinctet.Core.Cards.Base;
using Drinctet.Core.Parsing.TextDecoder;

namespace Drinctet.Presentation.Screen
{
    public interface IScreenGameManager : IGameManager
    {
        ITextDecoder TextDecoder { get; }
        void EnqueueFollowUp(IFollowUpSlidePresenter presenter, BaseCard card, TimeSpan timeSpan);
    }

    public class ScreenGameManager : GameManager, IScreenGameManager
    {
        private readonly ITextResource _textResource;

        public ScreenGameManager(DrinctetStatus status) : base(status)
        {
            _textResource = null;
        }

        public ISlidePresenter Next(CardsProvider cardsProvider)
        {
            var nextSlide = Selection.SelectNextSlide();
            var presenter = SlidePresenterFactory.Create(nextSlide, this, cardsProvider, _textResource);
            return presenter;
        }

        public ITextDecoder TextDecoder { get; }

        public void EnqueueFollowUp(IFollowUpSlidePresenter presenter, BaseCard card, TimeSpan timeSpan)
        {
            throw new NotImplementedException();
        }
    }
}