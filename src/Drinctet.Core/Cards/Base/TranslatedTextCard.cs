using System.Collections.Generic;

namespace Drinctet.Core.Cards.Base
{
    public abstract class TranslatedTextCard : BaseCard
    {
        protected TranslatedTextCard()
        {
            Translations = new List<TranslatedText>();
        }

        public IList<TranslatedText> Translations { get; }
    }
}