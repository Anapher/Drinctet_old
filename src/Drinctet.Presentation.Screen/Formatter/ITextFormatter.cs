﻿using System.Collections.Generic;
using System.Text;
using Drinctet.Core.Cards.Base;
using Drinctet.Core.Fragments;

namespace Drinctet.Presentation.Screen.Formatter
{
    public interface ITextFormatter
    {
        StringBuilder Format(IReadOnlyList<TextFragment> fragments, IReadOnlyList<PlayerSettings> playerSettings, IReadOnlyList<CardTag> tags);
    }
}