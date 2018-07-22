using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Drinctet.Core;
using Drinctet.Core.Cards.Base;

namespace Drinctet.Presentation.Screen.Slides
{
    public class DownPresenter : ISlidePresenter
    {
        public string Title { get; set; }
        public string Text { get; set; }

        public void Initialize(IScreenGameManager gameManager, ICardsProvider cardsProvider, ITextResource textResource)
        {
            Title = "DownPresenter.Title";

        }
    }

    public abstract class CardPresenter<TCard> : ISlidePresenter where TCard : BaseCard
    {
        public string Title { get; protected set; }
        public string Text { get; protected set; }

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

    public abstract class TextCardPresenter<TCard> : CardPresenter<TCard>, IFollowUpSlidePresenter where TCard : TextCard
    {
        protected override void Initialize()
        {
            if (SelectedCard.FollowUp?.Any() == true)
            {
                var shouldFollowUp = GameManager.Selection.TrueOrFalse(SelectedCard.FollowUpProbability);
                if (shouldFollowUp)
                    GameManager.EnqueueFollowUp(this, SelectedCard, SelectedCard.FollowUpDelay);
            }

            var content = GameManager.Selection.SelectRandomWeighted(SelectedCard.Content, element => element.Weight);
            SetText(content);
        }

        public void InitializeFollowUp(IScreenGameManager gameManager, BaseCard card, ITextResource textResource)
        {
            SelectedCard = (TCard) card;
            GameManager = gameManager;
            TextResource = textResource;

            var followUp =
                GameManager.Selection.SelectRandomWeighted(SelectedCard.FollowUp, element => element.Weight);
            SetText(followUp);
        }

        protected void SetText(TextElement textElement)
        {
            var translation = GetTranslation(textElement.Translations);
            var decoded = GameManager.TextDecoder.Decode(translation);

        }

        protected string GetTranslation(IReadOnlyDictionary<CultureInfo, string> texts)
        {
            if (texts.TryGetValue(TextResource.Culture, out var result))
                return result;

            if ((result = texts.FirstOrDefault(x =>
                    x.Key.TwoLetterISOLanguageName.StartsWith(TextResource.LanguageKey,
                        StringComparison.OrdinalIgnoreCase)).Value) != null)
                return result;

            if ((result = texts.FirstOrDefault(x =>
                    x.Key.TwoLetterISOLanguageName.StartsWith("en", StringComparison.OrdinalIgnoreCase)).Value) != null)
                return result;

            return texts.First().Value;
        }
    }
}