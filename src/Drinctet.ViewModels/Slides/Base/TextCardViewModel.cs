using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Drinctet.Core.Cards.Base;

namespace Drinctet.ViewModels.Slides.Base
{
    public abstract class TextCardViewModel<TCard> : CardViewModel<TCard> where TCard : TextCard
    {
        public override bool IsInteractive { get; } = false;
        public string Text { get; private set; }

        protected override void Initialize()
        {
            if (SelectedCard.FollowUp?.Any() == true)
            {
                var shouldFollowUp = GameManager.Selection.TrueOrFalse(SelectedCard.FollowUpProbability);
                if (shouldFollowUp)
                    GameManager.EnqueueFollowUp(this, SelectedCard, SelectedCard.FollowUpDelay);
            }

            var content = GameManager.Selection.SelectRandomWeighted(SelectedCard.Content, element => element.Weight);
            Text = FormatText(content).ToString();
        }

        protected virtual string GetText(TextElement textElement)
        {
            return GetTranslation(textElement.Translations);
        }

        protected virtual StringBuilder FormatText(TextElement textElement)
        {
            var translation = GetText(textElement);
            var decoded = GameManager.TextDecoder.Decode(translation);
            return GameManager.TextFormatter.Format(decoded, SelectedCard.Players, SelectedCard.Tags);
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
