using System;
using System.Collections.Generic;
using Drinctet.Core.Cards;
using Drinctet.Core.Cards.Base;
using Drinctet.ViewModels.Extensions;
using Drinctet.ViewModels.Manager;
using Drinctet.ViewModels.Slides.Base;
using Drinctet.ViewModels.Utilities;

namespace Drinctet.ViewModels.Slides
{
    public class QuestionViewModel : TextCardViewModel<QuestionCard>
    {
        private static readonly IReadOnlyList<Func<string, ITextResource, string>> Transformations =
            new Func<string, ITextResource, string>[] {
                (s, texts) =>  string.Format(texts["Question.Question1"], s.StartsWithLowerCase().HasTerminalCharacter()),
                (s, texts) => string.Format(texts["Question.Question2"], s.HasTerminalCharacter()),
                (s, texts) => string.Format(texts["Question.Question3"], s.HasTerminalCharacter()),
                (s, texts) => string.Format(texts["Question.Question4"], s.HasTerminalCharacter()),
                (s, texts) => string.Format(texts["Question.Question5"], s.HasTerminalCharacter()),
            };

        protected override string GetText(TextElement textElement)
        {
            var result = base.GetText(textElement);
            return Transformations[StaticRandom.Next(Transformations.Count)](result, TextResource);
        }
    }
}