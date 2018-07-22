using System.Collections.Generic;
using Drinctet.Core.Fragments;

namespace Drinctet.Core.Parsing.TextDecoder
{
    public interface ITextDecoder
    {
        IReadOnlyList<TextFragment> Decode(string s);
    }
}