namespace Drinctet.Core
{
    public class WeightedValue<TValue> : IWeighted
    {
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