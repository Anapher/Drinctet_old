using System;
using Drinctet.Core;
using Drinctet.Core.Cards.Base;
using Drinctet.Core.Parsing.TextDecoder;
using Drinctet.Presentation.Screen.Formatter;

namespace Drinctet.Presentation.Screen
{
    public interface IScreenGameManager : IGameManager
    {
        ITextDecoder TextDecoder { get; }
        ITextFormatter TextFormatter { get; }
        void EnqueueFollowUp(IFollowUpSlidePresenter presenter, BaseCard card, TimeSpan timeSpan);
    }
}