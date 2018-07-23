using System;
using System.Collections.Generic;
using System.Text;
using Drinctet.Core.Cards;
using Drinctet.Core.Cards.Base;
using Drinctet.Presentation.Screen.Extensions;
using Drinctet.Presentation.Screen.Slides.Base;
using Drinctet.Presentation.Screen.Utilities;

namespace Drinctet.Presentation.Screen.Slides
{
    public class WouldYouRatherPresenter : TextCardPresenter<WyrCard>
    {
        private static readonly IReadOnlyList<Func<string, ITextResource, string>> Transformations =
            new Func<string, ITextResource, string>[] {
                (s, texts) => string.Format(texts["WouldYouRather.Wyr1"], s),
                (s, texts) => string.Format(texts["WouldYouRather.Wyr2"], s),
                (s, texts) => string.Format(texts["WouldYouRather.Wyr3"], s),
                (s, texts) => string.Format(texts["WouldYouRather.Wyr4"], s),
            };
        
        protected override void Initialize()
        {
            base.Initialize();
            Title = TextResource["WouldYouRather.Title"];
        }

        protected override string GetText(TextElement textElement)
        {
            var result = base.GetText(textElement);
            return Transformations[StaticRandom.Next(Transformations.Count)](result, TextResource);
        }
    }
}