namespace Drinctet.Core
{
    public class NumberRange : INumber
    {
        public int Min { get; }
        public int Max { get; }

        public NumberRange(int min, int max)
        {
            Min = min;
            Max = max;
        }

        public int Count => Max - Min;
    }
}