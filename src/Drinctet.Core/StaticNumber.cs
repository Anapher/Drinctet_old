namespace Drinctet.Core
{
    public class StaticNumber : INumber
    {
        public StaticNumber(int number)
        {
            Number = number;
        }

        public int Number { get; }
        public int Count { get; } = 1;
    }
}