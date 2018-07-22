namespace Drinctet.Core
{
    public class WeightedValue<TValue> : IWeighted
    {
        public WeightedValue(TValue value, double weight)
        {
            Value = value;
            Weight = weight;
        }

        public WeightedValue(TValue value)
        {
            Value = value;
        }

        public WeightedValue()
        {
        }

        /// <summary>
        ///     The value
        /// </summary>
        public TValue Value { get; set; }

        /// <summary>
        ///     A value between zero and one that defines the weight of this value
        /// </summary>
        public double Weight { get; set; }
    }
}