using System;
using Drinctet.Core;

namespace Drinctet.ViewModels.Manager
{
    public interface ISlideViewModel
    {
        event EventHandler InvokeNextSlide;

        bool IsInteractive { get; }

        void Initialize(IScreenGameManager gameManager, ICardsProvider cardsProvider, ITextResource textResource);
    }
}