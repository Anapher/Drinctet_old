using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Drinctet.Core.Cards.Base;
using Drinctet.Core.Logging;
using Drinctet.Core.Parsing.Utilities;

namespace Drinctet.Core.Parsing
{
    public abstract class BaseCardParser<TCard> : ICardParser where TCard : BaseCard, new()
    {
        private static readonly ILog Logger = LogProvider.For<BaseCardParser<TCard>>();
        protected XmlReader Reader { get; set; }

        protected virtual IReadOnlyList<CardTag> BaseTags { get; }

        public BaseCard Parse(XmlReader xmlReader)
        {
            Logger.Trace("Start parsing card");
            Reader = xmlReader;

            var card = new TCard();

            ReadAttribute("willPower", s => card.WillPower = int.Parse(s));
            ReadAttribute("source", s => card.Source = s);
            ReadAttribute("tags", s =>
            {
                var tags = s.Split(',').Select(x => Enum.Parse<CardTag>(x.Trim(), true));
                if (BaseTags != null)
                    tags = tags.Concat(BaseTags);

                card.Tags = tags.ToList();
            });
            ReadAttribute("condition", s =>
            {
                //TODO
            });

            ParseAttributes(card);

            if (card.Tags == null && BaseTags != null)
                card.Tags = BaseTags;

            while (xmlReader.Read())
            {
                if (xmlReader.NodeType == XmlNodeType.Element)
                {
                    if (xmlReader.Name == $"{typeof(TCard).Name}.players")
                    {
                        card.Players = ParsePlayers(xmlReader.ReadSubtree()).ToList();
                        continue;
                    }

                    ParseElement(xmlReader.ReadSubtree(), card);
                }
            }

            OnCompleted(card);
            return card;
        }

        protected abstract bool ParseElement(XmlReader xmlReader, TCard card);
        protected abstract void ParseAttributes(TCard card);

        protected static void ReadAttribute(XmlReader reader, string name, Action<string> setValue)
        {
            var attribute = reader.GetAttribute(name);
            if (attribute != null)
            {
                setValue(attribute);
                Logger.Trace("Attribute {attributeName} set to {value}", name, attribute);
            }
            else
                Logger.Trace("The attribute {attributeName} was not found.", name);
        }

        protected void ReadAttribute(string name, Action<string> setValue) => ReadAttribute(Reader, name, setValue);

        protected static IEnumerable<PlayerSettings> ParsePlayers(XmlReader reader)
        {
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    var player = ParsePlayer(reader);
                    if (player != null)
                        yield return player;
                }
            }
        }

        protected virtual void OnCompleted(TCard card)
        {
        }

        protected static PlayerSettings ParsePlayer(XmlReader reader)
        {
            if (!ParserHelper.ParsePlayerTag(reader.Name, out var playerIndex))
            {
                Logger.Error("Error occurred when trying to parse '{name}' as player tag", reader.Name);
                return null;
            }

            var requiredGender = RequiredGender.None;

            var genderAttribute = reader.GetAttribute("gender");
            if (genderAttribute != null)
                if (!ParserHelper.ParseRequiredGender(genderAttribute, out requiredGender))
                    Logger.Error("Error occurred when trying to parse '{gender}' as gender",
                        genderAttribute);

            return new PlayerSettings
            {
                PlayerIndex = playerIndex,
                Gender = requiredGender
            };
        }
    }
}