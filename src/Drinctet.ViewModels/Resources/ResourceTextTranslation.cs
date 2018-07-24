using System;
using System.Globalization;
using Drinctet.ViewModels.Manager;

namespace Drinctet.ViewModels.Resources
{
    public class ResourceTextTranslation : ITextResource
    {
        public string LanguageKey { get; } = AppResources.Culture.TwoLetterISOLanguageName.Split('-')[0];
        public CultureInfo Culture { get; } = AppResources.Culture;

        public string this[string index] => AppResources.ResourceManager.GetString(index);
    }
}