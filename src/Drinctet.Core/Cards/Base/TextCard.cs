using System;
using System.Collections.Generic;

namespace Drinctet.Core.Cards.Base
{
    public abstract class TextCard : BaseCard
    {
        public virtual double FollowUpProbability { get; internal set; } = 1;
        public virtual TimeSpan FollowUpDelay { get; internal set; } = TimeSpan.Zero;

        public IReadOnlyList<TextElement> Content { get; internal set; }
        public IReadOnlyList<TextElement> FollowUp { get; internal set; }
    }
}