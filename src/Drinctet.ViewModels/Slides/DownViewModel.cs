using System;
using Drinctet.Core;
using Drinctet.Core.Cards;
using Drinctet.Core.Cards.Base;
using Drinctet.ViewModels.Manager;
using Drinctet.ViewModels.Slides.Base;

namespace Drinctet.ViewModels.Slides
{
    public class DownViewModel : ISlideViewModel
    {
        public string Text { get; set; } = "Was sind einige lustige und interessante Alternativen zum Krieg, mit denen die Länder ihre Differenzen beilegen könnten?";

        public event EventHandler InvokeNextSlide;
        public bool IsInteractive { get; }
        public void Initialize(IScreenGameManager gameManager, ICardsProvider cardsProvider, ITextResource textResource)
        {
            throw new NotImplementedException();
        }
    }
}