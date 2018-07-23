using System.Linq;
using Drinctet.Core;
using Drinctet.Core.Cards.Base;

namespace Drinctet.Presentation.Screen.Slides.Base
{
    public abstract class CardPresenter<TCard> : ISlidePresenter where TCard : BaseCard
    {
        public string Title { get; protected set; }
        public string Text { get; protected set; }
        public abstract bool IsInteractive { get; }

        protected TCard SelectedCard { get; set; }
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