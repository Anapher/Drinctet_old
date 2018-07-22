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

        public int PlayerIndex { get; internal set; }
        public RequiredGender Gender { get; internal set; }
    }
}