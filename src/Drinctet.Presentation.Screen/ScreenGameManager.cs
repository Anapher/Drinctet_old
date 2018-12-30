using System;
using Drinctet.Core;
using Drinctet.Core.Cards.Base;
using Drinctet.Core.Parsing.TextDecoder;
using Drinctet.Presentation.Screen.Formatter;

namespace Drinctet.Presentation.Screen
{
    public class ScreenGameManager : GameManager, IScreenGameManager
    {
        private readonly ITextResource _textResource;

        public ScreenGameManager(DrinctetStatus status, ITextResource textResource) : base(status)
        {
            _textResource = textResource;
            TextDecoder = new DefaultTextDecoder();
            TextFormatter = new TextFormatter(Selection, textResource) {BoldPlayerNames = true};
        }

        public ISlidePresenter Next(ICardsProvider cardsProvider)
        {
            var nextSlide = Selection.SelectNextSlide();
            return SlidePresenterFactory.Create(nextSlide, this, cardsProvider, _textResource);

        }

        public ITextDecoder TextDecoder { get; }
        public ITextFormatter TextFormatter { get; }

        public void EnqueueFollowUp(IFollowUpSlidePresenter presenter, BaseCard card, TimeSpan timeSpan)
        {
        }
    }
}