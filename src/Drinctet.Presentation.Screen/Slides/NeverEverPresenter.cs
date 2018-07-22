using System.Text;
using Drinctet.Core.Cards;
using Drinctet.Core.Cards.Base;
using Drinctet.Presentation.Screen.Slides.Base;

namespace Drinctet.Presentation.Screen.Slides
{
    public class NeverEverPresenter : TextCardPresenter<NeverEverCard>
    {
        protected override void Initialize()
        {
            base.Initialize();
            Title = TextResource["NeverEver.Title"];
        }

        protected override StringBuilder FormatText(TextElement textElement)
        {
            var result = base.FormatText(textElement);
            result.Insert(0, "...");
            return result;
        }
    }
}