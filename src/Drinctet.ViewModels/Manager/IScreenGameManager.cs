using System;
using Drinctet.Core;
using Drinctet.Core.Cards.Base;
using Drinctet.Core.Parsing.TextDecoder;
using Drinctet.ViewModels.Formatter;

namespace Drinctet.ViewModels.Manager
{
    public interface IScreenGameManager : IGameManager
    {
        ITextDecoder TextDecoder { get; }
        ITextFormatter TextFormatter { get; }
        void EnqueueFollowUp(ISlideViewModel viewModel, BaseCard card, TimeSpan timeSpan);
    }
}