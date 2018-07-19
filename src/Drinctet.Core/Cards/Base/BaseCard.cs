using System.Collections.Generic;

namespace Drinctet.Core.Cards.Base
{
    public abstract class BaseCard
    {
        public virtual int WillPower { get; internal set; }
        public virtual object Condition { get; internal set; }
        public virtual IReadOnlyList<PlayerSettings> Players { get; internal set; }
        public virtual bool IsPairing { get; internal set; }
    }
}