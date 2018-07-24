using System;
using Drinctet.Core;
using Drinctet.Core.Cards.Base;
using Drinctet.Core.Parsing.TextDecoder;
using Drinctet.ViewModels.Manager.Formatter;

namespace Drinctet.ViewModels.Manager
{
    public class ScreenGameManager : GameManager, IScreenGameManager
    {
        private readonly ITextResource _textResource;

        public ScreenGameManager(DrinctetStatus status, ITextResource textResource) : base(status)
        {
            _textResource = textResource;
            TextDecoder = new StringTextDecoder();
            TextFormatter = new TextFormatter(Selection, textResource) {BoldPlayerNames = true};
        }

        public ISlideViewModel Next(ICardsProvider cardsProvider)
        {
            var nextSlide = Selection.SelectNextSlide();
            var viewModel = SlideViewModelFactory.Create(nextSlide, this, cardsProvider, _textResource);
            return viewModel;
        }

        public ITextDecoder TextDecoder { get; }
        public ITextFormatter TextFormatter { get; }

        public void EnqueueFollowUp(ISlideViewModel viewModel, BaseCard card, TimeSpan timeSpan)
        {
        }
    }
}