using System.Xml;
using Drinctet.Core.Cards.Base;

namespace Drinctet.Core.Parsing
{
    public class TargetedTextCardParser<TCard> : TextCardParser<TCard> where TCard : TargetedTextCard, new()
    {
        protected override bool ParseElement(XmlReader xmlReader, TCard card)
        {
            if (base.ParseElement(xmlReader, card))
                return true;

            if (xmlReader.Name == $"{typeof(TCard).Name}.targetPlayer")
            {
                xmlReader.Read();
                card.TargetPlayer = ParsePlayer(xmlReader);
                return true;
            }

            return false;
        }
    }
}