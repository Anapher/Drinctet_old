namespace Drinctet.Core
{
    public class PlayerInfo
    {
        public PlayerInfo(int id, Gender gender)
        {
            Id = id;
            Gender = gender;
        }

        public PlayerInfo()
        {
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public Gender Gender { get; set; }

        public override string ToString() => $"{Name} ({Id})";
    }
}