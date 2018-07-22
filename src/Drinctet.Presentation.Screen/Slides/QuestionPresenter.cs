using System;
using System.Collections.Generic;
using Drinctet.Core.Cards;
using Drinctet.Core.Cards.Base;
using Drinctet.Presentation.Screen.Extensions;
using Drinctet.Presentation.Screen.Slides.Base;
using Drinctet.Presentation.Screen.Utilities;

namespace Drinctet.Presentation.Screen.Slides
{
    public class QuestionPresenter : TextCardPresenter<QuestionCard>
    {
        private static readonly IReadOnlyList<Func<string, ITextResource, string>> Transformations =
            new Func<string, ITextResource, string>[] {
                (s, texts) =>  string.Format(texts["Question.Question1"], s.StartsWithLowerCase()),
                (s, texts) => string.Format(texts["Question.Question2"], s),
                (s, texts) => string.Format(texts["Question.Question3"], s),
            };

        protected override void Initialize()
        {
            base.Initialize();
            Title = TextResource["Question.Title"];
        }

        protected override string GetText(TextElement textElement)
        {
            var result = base.GetText(textElement);
            return Transformations[StaticRandom.Next(Transformations.Count)](result, TextResource);
        }
    }
}