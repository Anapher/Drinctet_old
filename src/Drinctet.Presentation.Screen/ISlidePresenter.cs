using System.Globalization;
using Drinctet.Core;
using Drinctet.Core.Cards.Base;

namespace Drinctet.Presentation.Screen
{
    public interface ISlidePresenter
    {
        string Title { get; }
        string Text { get; }

        void Initialize(IScreenGameManager gameManager, ICardsProvider cardsProvider, ITextResource textResource);
    }

    public interface IFollowUpSlidePresenter
    {
        void InitializeFollowUp(IScreenGameManager gameManager, BaseCard card, ITextResource textResource);
    }

    public interface ITextResource
    {
        string LanguageKey { get; }
        CultureInfo Culture { get; }

        string this[string index] { get; }
    }
}