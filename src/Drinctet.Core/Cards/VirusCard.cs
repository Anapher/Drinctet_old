﻿using System;
using Drinctet.Core.Cards.Base;

namespace Drinctet.Core.Cards
{
    public class VirusCard : TargetedTextCard
    {
        public override TimeSpan FollowUpDelay { get; internal set; } = TimeSpan.FromMinutes(5);
    }
}