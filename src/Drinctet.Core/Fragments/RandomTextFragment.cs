﻿using System.Collections.Generic;

namespace Drinctet.Core.Fragments
{
    public class RandomTextFragment : TextFragment
    {
        public IReadOnlyList<string> Texts { get; set; }

        public override string ToString() => $"!{{{string.Join(",", Texts)}}}";
    }
}