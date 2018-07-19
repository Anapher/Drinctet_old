using System.Xml;
using Drinctet.Core.Cards.Base;

namespace Drinctet.Core.Parsing
{
    public abstract class BaseCardParser<TCard> : ICardParser where TCard : BaseCard, new()
    {
        public BaseCard Parse(XmlReader xmlReader)
        {
            var card = new TCard();

            var willPower = xmlReader.GetAttribute("willPower");
            if (willPower != null)
            {
                card.WillPower = int.Parse(willPower);
            }

            var condition = xmlReader.GetAttribute("condition");
            if (condition != null)
            {
                //TODO: parse condition
            }

            Parse(xmlReader, card);
            return card;
        }

        protected abstract void Parse(XmlReader xmlReader, TCard card);
    }
}