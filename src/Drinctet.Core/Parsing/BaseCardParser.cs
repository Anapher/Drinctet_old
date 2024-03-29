﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Drinctet.Core.Cards.Base;
using Drinctet.Core.Logging;
using Drinctet.Core.Parsing.Utilities;
using HashidsNet;

namespace Drinctet.Core.Parsing
{
    public abstract class BaseCardParser<TCard> : ICardParser where TCard : BaseCard, new()
    {
        private static readonly ILog Logger = LogProvider.For<BaseCardParser<TCard>>();
        protected XmlReader Reader { get; set; }

        protected virtual IReadOnlyList<CardTag> BaseTags { get; }

        public BaseCard Parse(XmlReader xmlReader, string source)
        {
            Logger.Trace("Start parsing card");
            Reader = xmlReader;

            var card = new TCard();

            ReadAttribute("willPower", s => card.WillPower = int.Parse(s));
            ReadAttribute("id", s =>
            {
                card.Origin = new CardOrigin {CardId = s, SourceString = source};

                if (int.TryParse(s, out var idInteger))
                    card.Id = idInteger * source.GetHashCode();

                var hashIds = new Hashids("DrinctetByVG");
                var result = hashIds.Decode(s);
                card.Id = result[0] * 1000 + result[1];
            });
            ReadAttribute("tags", s =>
            {
                var tags = ParserHelper.ParseEnum<CardTag>(s);
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
                    var reader = xmlReader.ReadSubtree();
                    reader.Read();

                    if (xmlReader.Name == $"{typeof(TCard).Name}.players")
                    {
                        card.Players = ParsePlayers(reader).ToList();
                        continue;
                    }

                    ParseElement(reader, card);
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