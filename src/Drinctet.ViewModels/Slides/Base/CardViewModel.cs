using System;
using System.Linq;
using Drinctet.Core;
using Drinctet.Core.Cards.Base;
using Drinctet.ViewModels.Manager;

namespace Drinctet.ViewModels.Slides.Base
{
    public abstract class CardViewModel<TCard> : ISlideViewModel where TCard : BaseCard
    {
        public event EventHandler InvokeNextSlide;

        public abstract bool IsInteractive { get; }

        public TCard SelectedCard { get; set; }
        protected IScreenGameManager GameManager { get; set; }
        protected ITextResource TextResource { get; set; }

        public virtual void Initialize(IScreenGameManager gameManager, ICardsProvider cardsProvider, ITextResource textResource)
        {
            SelectedCard = gameManager.Selection.SelectCard(cardsProvider.Cards.OfType<TCard>().ToList());
            GameManager = gameManager;
            TextResource = textResource;

            Initialize();
        }

        protected abstract void Initialize();
    }
}