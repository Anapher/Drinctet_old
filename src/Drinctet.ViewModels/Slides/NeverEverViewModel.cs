using System.Text;
using Drinctet.Core.Cards;
using Drinctet.Core.Cards.Base;
using Drinctet.ViewModels.Slides.Base;

namespace Drinctet.ViewModels.Slides
{
    public class NeverEverViewModel : TextCardViewModel<NeverEverCard>
    {
        protected override StringBuilder FormatText(TextElement textElement)
        {
            var result = base.FormatText(textElement);
            result.Insert(0, "...");
            return result;
        }
    }
}