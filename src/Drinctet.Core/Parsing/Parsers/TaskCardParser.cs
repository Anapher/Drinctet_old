using System;
using System.Xml;
using Drinctet.Core.Cards;

namespace Drinctet.Core.Parsing.Parsers
{
    internal class TaskCardParser : TextCardParser<TaskCard>
    {
        protected override void ParseAttributes(TaskCard card)
        {
            base.ParseAttributes(card);

            var categoryAttribute = Reader.GetAttribute("category");
            if (categoryAttribute != null)
                card.Category = Enum.Parse<TaskCategory>(categoryAttribute, true);
        }

        protected override bool ParseElement(XmlReader xmlReader, TaskCard card)
        {
            if (base.ParseElement(xmlReader, card))
                return true;

            if (xmlReader.Name == "TaskCard.targetPlayer")
            {
                xmlReader.Read();
                card.TargetPlayer = ParsePlayer(xmlReader);
                return true;
            }

            return false;
        }
    }
}