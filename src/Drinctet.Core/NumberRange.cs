namespace Drinctet.Core
{
    public class NumberRange : INumber
    {
        public int Min { get; internal set; }
        public int Max { get; internal set; }

        internal NumberRange()
        {
        }

        public NumberRange(int min, int max)
        {
            Min = min;
            Max = max;
        }

        public int Count => Max - Min;

        public override string ToString() => Min + "-" + Max;
    }
}