using System.Collections.Generic;
using System.Globalization;

namespace Drinctet.Core.Cards.Base
{
    public class TextElement
    {
        public double Weight { get; internal set; } = 1;
        public IReadOnlyDictionary<CultureInfo, string> Translations { get; internal set; }
    }
}