using Drinctet.Core.Cards;
using Drinctet.Presentation.Screen.Slides.Base;

namespace Drinctet.Presentation.Screen.Slides
{
    public class DownPresenter : TextCardPresenter<DownCard>
    {
        protected override void Initialize()
        {
            base.Initialize();

            Title = TextResource["DownPresenter.Title"];
        }
    }
}