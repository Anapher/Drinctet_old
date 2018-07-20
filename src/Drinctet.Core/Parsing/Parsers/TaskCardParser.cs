using System;
using Drinctet.Core.Cards;

namespace Drinctet.Core.Parsing.Parsers
{
    internal class TaskCardParser : TargetedTextCardParser<TaskCard>
    {
        protected override void ParseAttributes(TaskCard card)
        {
            base.ParseAttributes(card);

            var categoryAttribute = Reader.GetAttribute("category");
            if (categoryAttribute != null)
                card.Category = Enum.Parse<TaskCategory>(categoryAttribute, true);
        }
    }
}