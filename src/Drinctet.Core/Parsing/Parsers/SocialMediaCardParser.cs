using System.Collections.Generic;
using Drinctet.Core.Cards;
using Drinctet.Core.Cards.Base;

namespace Drinctet.Core.Parsing.Parsers
{
    internal class SocialMediaCardParser : TargetedTextCardParser<SocialMediaCard>
    {
        protected override IReadOnlyList<CardTag> BaseTags { get; } = new[] {CardTag.SocialMedia};
    }
}