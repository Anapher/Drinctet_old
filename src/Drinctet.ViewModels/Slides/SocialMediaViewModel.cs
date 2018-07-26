using System;
using System.Linq;
using Drinctet.Core.Cards;
using Drinctet.Core.Cards.Base;
using Drinctet.ViewModels.Slides.Base;

namespace Drinctet.ViewModels.Slides
{
    public class SocialMediaViewModel : TextCardViewModel<SocialMediaCard>
    {
        protected override string GetText(TextElement textElement)
        {
            var result = base.GetText(textElement);
            var words = result.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => "\"" + x.Trim() + "\"").ToArray();
            var text = string.Join(", ", words.Take(words.Length - 1));

            var template = TextResource["SocialMedia.Text"];
            return string.Format(template, GameManager.Status.SocialMediaPlatform, text, words.Last());
        }
    }
}