using System;
using System.Globalization;
using System.Xml;
using Drinctet.Core.Cards.Base;

namespace Drinctet.Core.Parsing
{
    public abstract class TranslatedCardParser<TCard> : BaseCardParser<TCard> where TCard : TranslatedTextCard, new()
    {
        protected override void Parse(XmlReader xmlReader, TCard card)
        {
            ParseAttributes(xmlReader, card);
            
            while (xmlReader.Read())
            {
                switch (xmlReader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (xmlReader.Name != "Text")
                            throw new InvalidOperationException(
                                $"Invalid element detected when parsing xml card {typeof(TCard)}: '{xmlReader.Name}'. Expected 'Text'");

                        var culture = xmlReader.GetAttribute("lang");
                        if (culture == null)
                            throw new InvalidOperationException("The translation does not have a 'lang' attribute.");
                        var content = xmlReader.ReadElementContentAsString();
                        card.Translations.Add(new TranslatedText(content, CultureInfo.GetCultureInfo(culture)));
                        break;
                    case XmlNodeType.EndElement:
                        return;
                }
            }
        }

        protected virtual void ParseAttributes(XmlReader xmlReader, TCard card) { }
    }
}