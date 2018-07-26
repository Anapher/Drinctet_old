using System;
using System.Collections.Generic;
using Drinctet.Core.Cards;
using Drinctet.Core.Cards.Base;
using Drinctet.ViewModels.Manager;
using Drinctet.ViewModels.Slides.Base;
using Drinctet.ViewModels.Utilities;

namespace Drinctet.ViewModels.Slides
{
    public class WouldYouRatherViewModel : TextCardViewModel<WyrCard>
    {
        private static readonly IReadOnlyList<Func<string, ITextResource, string>> Transformations =
            new Func<string, ITextResource, string>[] {
                (s, texts) => string.Format(texts["WouldYouRather.Wyr1"], s),
                (s, texts) => string.Format(texts["WouldYouRather.Wyr2"], s),
                (s, texts) => string.Format(texts["WouldYouRather.Wyr3"], s),
                (s, texts) => string.Format(texts["WouldYouRather.Wyr4"], s),
            };

        protected override string GetText(TextElement textElement)
        {
            var result = base.GetText(textElement);
            return Transformations[StaticRandom.Next(Transformations.Count)](result, TextResource);
        }
    }
}