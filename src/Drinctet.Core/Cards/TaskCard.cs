using Drinctet.Core.Cards.Base;

namespace Drinctet.Core.Cards
{
    public class TaskCard : TextCard
    {
        public PlayerSettings TargetPlayer { get; set; }
        public TaskCategory Category { get; set; }
    }

    public enum TaskCategory
    {
        Dare
    }
}