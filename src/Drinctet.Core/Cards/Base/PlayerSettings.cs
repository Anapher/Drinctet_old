namespace Drinctet.Core.Cards.Base
{
    public class PlayerSettings
    {
        public PlayerSettings(int playerIndex, RequiredGender gender)
        {
            PlayerIndex = playerIndex;
            Gender = gender;
        }

        public PlayerSettings()
        {
        }

        public int PlayerIndex { get; internal set; } = 1;
        public RequiredGender Gender { get; internal set; }
    }
}