using System;
using System.Collections.Generic;
using System.Linq;
using Drinctet.Core;
using Drinctet.Core.Cards;
using Drinctet.Core.Cards.Base;
using Drinctet.ViewModels.Extensions;
using Drinctet.ViewModels.Manager;
using Drinctet.ViewModels.Slides.Base;

namespace Drinctet.ViewModels.Slides
{
    public class QuestionViewModel : TextCardViewModel<QuestionCard>
    {
        private static readonly IReadOnlyList<TextTransformation> Transformations = new[]
        {
            new TextTransformation(6, (s, texts) => string.Format(texts["Question.Question1"], s.StartsWithLowerCase().HasTerminalCharacter())), 
            new TextTransformation(2, (s, texts) => string.Format(texts["Question.Question2"], s.HasTerminalCharacter()), (card, status) => status.Players.Count > 1), 
            new TextTransformation(1, (s, texts) => string.Format(texts["Question.Question3"], s.HasTerminalCharacter())),
            new TextTransformation(2, (s, texts) => string.Format(texts["Question.Question4"], s.HasTerminalCharacter()), (card, status) => card.Tags?.Any(x => x == CardTag.Personal || x == CardTag.Deep) == true && status.Players.Count > 1&& status.WillPower > 4),
            new TextTransformation(2, (s, texts) => string.Format(texts["Question.Question5"], s.HasTerminalCharacter().StartsWithLowerCase()), (card, status) => status.Players.Count > 1 && status.WillPower > 4),
            new TextTransformation(12, (s, texts) => string.Format(texts["Question.Question6"], s.HasTerminalCharacter())),
        };

        protected override string GetText(TextElement textElement)
        {
            var result = base.GetText(textElement);

            return GameManager.Selection
                .SelectRandomWeighted(
                    Transformations.Where(x => x.Condition(SelectedCard, GameManager.Status)).ToList())
                .Transform(result, TextResource);
        }
    }

    public class TextTransformation : IWeighted
    {
        public TextTransformation(double weight, Func<string, ITextResource, string> transform, Func<BaseCard, DrinctetStatus, bool> condition)
        {
            Weight = weight;
            Transform = transform;
            Condition = condition;
        }

        public TextTransformation(double weight, Func<string, ITextResource, string> transform) : this(weight, transform, (card, status) => true)
        {
        }

        public double Weight { get; }
        public Func<string, ITextResource, string> Transform { get; }
        public Func<BaseCard, DrinctetStatus, bool> Condition { get; }
    }
}