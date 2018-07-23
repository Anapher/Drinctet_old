using Drinctet.Core;

namespace Drinctet.Presentation.Screen
{
    public interface ISlidePresenter
    {
        string Title { get; }
        string Text { get; }
        bool IsInteractive { get; }

        void Initialize(IScreenGameManager gameManager, ICardsProvider cardsProvider, ITextResource textResource);
    }
}