﻿using System.Collections.Generic;

namespace Drinctet.Core.Fragments
{
    public class RandomNumberFragment : TextFragment
    {
        public IReadOnlyList<INumber> Numbers { get; set; }

        public override string ToString() => $"!{{{string.Join(",", Numbers)}}}";
    }
}