using System.Globalization;

namespace Drinctet.ViewModels.Manager
{
    public interface ITextResource
    {
        string LanguageKey { get; }
        CultureInfo Culture { get; }

        string this[string index] { get; }
    }
}