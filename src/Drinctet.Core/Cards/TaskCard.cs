using Drinctet.Core.Cards.Base;

namespace Drinctet.Core.Cards
{
    public class TaskCard : TargetedTextCard
    {
        public TaskCategory Category { get; set; }
    }

    public enum TaskCategory
    {
        Dare
    }
}