using System;
using System.Collections.Generic;
using System.Linq;
using Drinctet.Core.Cards.Base;

namespace Drinctet.Core.Selection
{
    public abstract class SelectionAlgorithmBase : ISelectionAlgorithm
    {
        protected DrinctetStatus Status;
        protected Random Random;

        public void Initialize(DrinctetStatus status, Random random)
        {
            Status = status;
            Random = random;
        }

        public abstract IReadOnlyList<PlayerInfo> SelectPlayers(IReadOnlyList<RequiredGender> requiredGenders,
            IReadOnlyList<CardTag> tags);

        public abstract TCard SelectCard<TCard>(IReadOnlyList<TCard> cards) where TCard : BaseCard;

        public abstract int GetWillPowerLevel();

        public abstract int GetSips(int min);

        public abstract SlideType SelectNextSlide();

        public int SelectRandomNumber(IReadOnlyList<INumber> numbers)
        {
            var number = SelectRandomWeighted(numbers, n => n.Count);
            if (number is StaticNumber staticNumber)
                return staticNumber.Number;

            var range = (NumberRange)number;
            return Random.Next(range.Min, range.Max);
        }

        public string SelectRandomString(IReadOnlyList<string> values)
        {
            return values[Random.Next(values.Count)];
        }

        public int GetRandomNumber(int minValue, int maxValue) => Random.Next(minValue, maxValue);
        public int GetRandomNumber(int maxValue) => Random.Next(maxValue);

        public bool TrueOrFalse(double trueProbability)
        {
            var value = Random.NextDouble();
            if (value < trueProbability)
                return true;

            return false;
        }

        //https://stackoverflow.com/a/3899874
        public T SelectRandomWeighted<T>(IReadOnlyList<T> items) where T : IWeighted
        {
            if (items.Count == 0)
                return default;

            var totalWeight = items.Sum(x => x.Weight);
            var randomNum = Random.NextDouble() * totalWeight;

            foreach (var item in items)
            {
                if (randomNum < item.Weight)
                    return item;

                randomNum = randomNum - item.Weight;
            }

            throw new InvalidOperationException("No choice could be made");
        }

        public T SelectRandomWeighted<T>(IReadOnlyList<T> items, Func<T, double> getWeight)
        {
            if (items.Count == 0)
                return default;

            var weights = items.ToDictionary(x => x, getWeight);
            var totalWeight = weights.Sum(x => x.Value);
            var randomNum = Random.NextDouble() * totalWeight;

            foreach (var item in items)
            {
                var weight = weights[item];

                if (randomNum < weight)
                    return item;

                randomNum = randomNum - weight;
            }

            return default;
            //throw new InvalidOperationException("No choice could be made");
        }
    }
}