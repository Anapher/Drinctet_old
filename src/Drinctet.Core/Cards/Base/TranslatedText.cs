using System.Globalization;

namespace Drinctet.Core.Cards.Base
{
    public class TranslatedText
    {
        public TranslatedText(string content, CultureInfo culture)
        {
            Content = content;
            Culture = culture;
        }

        public string Content { get; set; }
        public CultureInfo Culture { get; set; }
    }
}