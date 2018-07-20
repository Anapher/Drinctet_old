using Drinctet.Core.Cards.Base;

namespace Drinctet.Core.Cards
{
    public class QuestionCard : TargetedTextCard
    {
        public QuestionCategory Category { get; internal set; }
    }

    public enum QuestionCategory
    {
        Default,
        ConversationStarter,
        Truth,
        DeepTalk,
        Funny,
        PersonalGirl,
        PersonalGuy
    }
}