using System;
using System.Collections.Generic;
using Drinctet.Core.Cards;
using Drinctet.Core.Cards.Base;
using Drinctet.Presentation.Screen.Extensions;
using Drinctet.Presentation.Screen.Slides.Base;
using Drinctet.Presentation.Screen.Utilities;

namespace Drinctet.Presentation.Screen.Slides
{
    public class TaskPresenter : TextCardPresenter<TaskCard>
    {
        private static readonly IReadOnlyList<Func<string, ITextResource, string>> Transformations =
            new Func<string, ITextResource, string>[] {
                (s, texts) => string.Format(texts["Task.Task1"], s.StartsWithLowerCase()),
                (s, texts) => string.Format(texts["Task.Task2"], s.StartsWithLowerCase()),
                (s, texts) => string.Format(texts["Task.Task3"], s.StartsWithLowerCase()),
            };

        protected override void Initialize()
        {
            base.Initialize();
            Title = TextResource["Task.Title"];
        }

        protected override string GetText(TextElement textElement)
        {
            var result = base.GetText(textElement);
            return Transformations[StaticRandom.Next(Transformations.Count)](result, TextResource);
        }
    }
}