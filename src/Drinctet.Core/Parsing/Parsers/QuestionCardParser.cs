using System;
using Drinctet.Core.Cards;
using Drinctet.Core.Cards.Base;

namespace Drinctet.Core.Parsing.Parsers
{
    internal class QuestionCardParser : TargetedTextCardParser<QuestionCard>
    {
        protected override void ParseAttributes(QuestionCard card)
        {
            base.ParseAttributes(card);

            var categoryAttribute = Reader.GetAttribute("category");
            if (categoryAttribute != null)
                card.Category = Enum.Parse<QuestionCategory>(categoryAttribute, true);
        }

        protected override void OnCompleted(QuestionCard card)
        {
            base.OnCompleted(card);

            if (card.Category == QuestionCategory.PersonalGirl)
            {
                if (card.TargetPlayer == null)
                    card.TargetPlayer = new PlayerSettings {Gender = RequiredGender.Female};
                else card.TargetPlayer.Gender = RequiredGender.Female;
            }
            else if (card.Category == QuestionCategory.PersonalGuy)
            {
                if (card.TargetPlayer == null)
                    card.TargetPlayer = new PlayerSettings {Gender = RequiredGender.Male};
                else card.TargetPlayer.Gender = RequiredGender.Female;
            }
        }
    }
}