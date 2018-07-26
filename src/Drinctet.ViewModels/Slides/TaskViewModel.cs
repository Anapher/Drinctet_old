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
    public class TaskViewModel : TextCardViewModel<TaskCard>
    {
        private static readonly IReadOnlyList<Func<string, ITextResource, string>> Transformations =
            new Func<string, ITextResource, string>[] {
                (s, texts) => string.Format(texts["Task.Task1"], s.StartsWithLowerCase().NoTerminalPoint()),
                (s, texts) => string.Format(texts["Task.Task2"], s.StartsWithLowerCase().NoTerminalPoint()),
                (s, texts) => string.Format(texts["Task.Task3"], s.StartsWithLowerCase().NoTerminalPoint()),
            };

        protected override string GetText(TextElement textElement)
        {
            var result = base.GetText(textElement);
            return Transformations[StaticRandom.Next(Transformations.Count)](result, TextResource);
        }
    }
}