using System;
using System.Collections.Generic;
using System.Text;

namespace Drinctet.Core.Cards.Base
{
    public abstract class TargetedTextCard : TextCard
    {
        public PlayerSettings TargetPlayer { get; set; }
    }
}
