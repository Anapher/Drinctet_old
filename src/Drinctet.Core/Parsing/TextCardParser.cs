using System;
using System.Globalization;
using System.Xml;
using Drinctet.Core.Cards.Base;

namespace Drinctet.Core.Parsing
{
    public abstract class TextCardParser<TCard> : BaseCardParser<TCard> where TCard : TextCard, new()
    {
        private readonly TextContentParser _contentParser = new TextContentParser();

        protected override void ParseAttributes(TCard card)
        {
            ReadAttribute("followUpProbability", s => card.FollowUpProbability = double.Parse(s, CultureInfo.InvariantCulture));
            ReadAttribute("followUpDelay", s => card.FollowUpDelay = TimeSpan.Parse(s));
        }

        protected override bool ParseElement(XmlReader xmlReader, TCard card)
        {
            switch (xmlReader.Name)
            {
                case "Case":
                case "Text":
                    return _contentParser.AddElement(xmlReader);
            }

            if (xmlReader.Name == $"{typeof(TCard).Name}.followUp")
            {
                var parser = new TextContentParser();
                while (xmlReader.Read())
                {
                    if (xmlReader.NodeType == XmlNodeType.Element)
                        parser.AddElement(xmlReader);
                }

                card.FollowUp = parser.Result;
                return true;
            }

            return false;
        }
    }
}