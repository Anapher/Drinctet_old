using System.Globalization;

namespace Drinctet.Presentation.Screen
{
    public interface ITextResource
    {
        string LanguageKey { get; }
        CultureInfo Culture { get; }

        string this[string index] { get; }
    }
}