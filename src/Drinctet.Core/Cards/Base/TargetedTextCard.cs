namespace Drinctet.Core.Cards.Base
{
    public abstract class TargetedTextCard : TextCard
    {
        public PlayerSettings TargetPlayer { get; set; }
    }
}