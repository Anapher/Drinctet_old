using System;
using System.Collections.Generic;
using System.Linq;
using Drinctet.Core.Cards.Base;

namespace Drinctet.Core.Selection
{
    public class BenokolaAlgorithm : ISelectionAlgorithm
    {
        private Random _random;
        private DrinctetStatus _status;

        public int MaxRecentSlides { get; set; } = 20;

        public void Initialize(DrinctetStatus status, Random random)
        {
            _random = random;
            _status = status;
        }
        
        public IReadOnlyList<PlayerInfo> SelectPlayers(IReadOnlyList<RequiredGender> requiredGenders,
            IReadOnlyList<CardTag> tags)
        {
            if (requiredGenders.Count == 0)
                return Array.Empty<PlayerInfo>();

            int lowestPlayerScore = _status.PlayerScores.Min(x => x.Value);
            int greatestDifference = lowestPlayerScore - _status.PlayerScores.Max(x => x.Value);

            var result = new PlayerInfo[requiredGenders.Count];
            var forArrangement = new List<PlayerInfo>();
            var playerGenders = requiredGenders.ToArray();
            var resultCounter = 0;

            while (result.Any(x => x == null))
            {
                for (int i = 0; i < playerGenders.Length; i++)
                {
                    if (result[i] != null)
                        continue;

                    var gender = playerGenders[i];

                    IEnumerable<PlayerInfo> sourceList;
                    switch (gender)
                    {
                        case RequiredGender.None:
                            sourceList = _status.Players;
                            break;
                        case RequiredGender.Male:
                            sourceList = _status.Players.Where(x => x.Gender == Gender.Male);
                            break;
                        case RequiredGender.Female:
                            sourceList = _status.Players.Where(x => x.Gender == Gender.Female);
                            break;
                        default:
                            continue;
                    }

                    var source = sourceList.Where(x => Array.IndexOf(result, x) == -1).ToList();
                    var player = SelectRandomWeighted(source, p =>
                    {
                        double value;
                        if (greatestDifference == 0)
                            value = 1;
                        else
                            value = 1.2 - (_status.PlayerScores[p.Id] - lowestPlayerScore) /
                                    (double) greatestDifference;

                        if (forArrangement.Contains(p))
                        {
                            value += _status.Players.Count - 1; // 50/50 that the couple is selected

                            if (tags?.Contains(CardTag.Pairing) == true)
                            {
                                value += _status.Players.Count - 1; // 2/3

                                if (tags.Contains(CardTag.Sexual))
                                    value += _status.Players.Count - 1; //over 90000
                            }
                        }

                        return value;
                    });

                    result[i] = player;
                    resultCounter++;
                    _status.PlayerScores[player.Id]++;

                    if (_status.PlayerArrangements.TryGetValue(player.Id, out var arranging))
                        forArrangement.Add(_status.Players.First(x => x.Id == arranging));
                }

                if (resultCounter == result.Length)
                    break;

                var dominant =
                    result.Where(x => x != null).GroupBy(x => x.Gender).OrderByDescending(x => x.Count())
                        .FirstOrDefault()?.Key;

                if (dominant == null) //one gender must be selected
                {
                    //we try to change a 'Same' gender, so Same and opposite stay on different sites
                    var changed = false;
                    for (int i = 0; i < playerGenders.Length; i++)
                    {
                        if (playerGenders[i] == RequiredGender.Same)
                        {
                            playerGenders[i] = RequiredGender.None;
                            changed = true;
                            break;
                        }
                    }

                    if (!changed) //fml
                        playerGenders[0] = RequiredGender.None;
                    continue;
                }
                
                for (int i = 0; i < playerGenders.Length; i++)
                {
                    var gender = playerGenders[i];

                    switch (gender)
                    {
                        case RequiredGender.Opposite:
                            if (dominant == Gender.Female)
                                playerGenders[i] = RequiredGender.Male;
                            else playerGenders[i] = RequiredGender.Female;
                            break;
                        case RequiredGender.Same:
                            if (dominant == Gender.Female)
                                playerGenders[i] = RequiredGender.Female;
                            else playerGenders[i] = RequiredGender.Male;
                            break;
                        default:
                            continue;
                    }
                }
            }

            return result;
        }

        public int SelectRandomNumber(IReadOnlyList<INumber> numbers)
        {
            var number = SelectRandomWeighted(numbers, n => n.Count);
            if (number is StaticNumber staticNumber)
                return staticNumber.Number;

            var range = (NumberRange) number;
            return _random.Next(range.Min, range.Max);
        }

        public string SelectRandomString(IReadOnlyList<string> values)
        {
            return values[_random.Next(values.Count)];
        }

        public int GetWillPowerLevel()
        {
            var willPower = 0;
            if (_status.StaticWillPower == null)
            {
                var localTime = DateTimeOffset.UtcNow.Add(_status.TimeZone.BaseUtcOffset);
                if (localTime.Hour > 22 || localTime.Hour < 12)
                    willPower++;

                if (localTime.Hour < 12)
                    willPower++;

                willPower += (int)(localTime - _status.StartTime).TotalHours;
                willPower += _status.SlidesCounter / 12;
                willPower = (int)(willPower * _status.WillPowerMultiplicator);
            }
            else willPower = _status.StaticWillPower.Value;

            if (willPower > 10)
                willPower = 10;

            return willPower;
        }

        public TCard SelectCard<TCard>(IReadOnlyList<TCard> cards) where TCard : BaseCard
        {
            var willPower = GetWillPowerLevel();

            var factor = Math.Max(_status.CardHistory.Count, 10);
            var cardsScore = cards.ToDictionary(x => x, x => factor);
            var willPowerDistribution = new Dictionary<int, int>();

            foreach (var card in cards)
            {
                if (willPowerDistribution.TryGetValue(card.WillPower, out var count))
                    willPowerDistribution[card.WillPower] = count + 1;
                else willPowerDistribution[card.WillPower] = 1;

                if (card is TextCard textCard)
                {
                    if (!textCard.Content.Any(x =>
                        x.Translations.Any(y => y.Key.TwoLetterISOLanguageName.StartsWith(_status.Language))))
                        cardsScore[card] -= factor / 2;
                }
            }

            for (int i = _status.CardHistory.Count - 1; i >= 0; i--)
            {
                var id = _status.CardHistory[i];
                var card = cards.FirstOrDefault(x => x.Id == id);
                if (card != null)
                    cardsScore[card] -= i;
            }

            var result = SelectRandomWeighted(cards, card =>
            {
                var weight = cardsScore[card];
                if (weight < 0)
                    return 0;

                var diff = willPower - card.WillPower;
                if (diff < -3) //we dont want to go that far forward
                    return 0;

                if (diff == 0)
                    weight += factor;

                if (diff < 0) //forward
                    diff = Math.Abs(diff);

                weight += factor / (diff + 1);

                //if (card.Tags.Any())
                //{
                //    var min = card.Tags.Select(x => _status.Tags.FirstOrDefault(y => y.Value == x)?.Weight ?? 0).Min();
                //    weight *= min;
                //}

                return (double) weight / willPowerDistribution[card.WillPower];
            });

            _status.CardHistory.Add(result.Id);
            return result;
        }

        public int GetRandomNumber(int minValue, int maxValue) => _random.Next(minValue, maxValue);
        public int GetRandomNumber(int maxValue) => _random.Next(maxValue);

        public bool TrueOrFalse(double trueProbability)
        {
            var value = _random.NextDouble();
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
            var randomNum = _random.NextDouble() * totalWeight;

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
            var randomNum = _random.NextDouble() * totalWeight;

            foreach (var item in items)
            {
                var weight = weights[item];

                if (randomNum < weight)
                    return item;

                randomNum = randomNum - weight;
            }

            throw new InvalidOperationException("No choice could be made");
        }

        public int GetSips(int min)
        {
            var baseSips = (int) (_status.DrinkALot * 4);

            var timespan = DateTimeOffset.UtcNow - _status.StartTime;

            if ((int) Math.Floor(timespan.TotalHours) % 2 == 0)
                baseSips += 1;

            var localTime = DateTimeOffset.UtcNow.Add(_status.TimeZone.BaseUtcOffset);
            if (localTime.Hour < 22 && localTime.Hour > 12)
                baseSips += 1;

            var sips = baseSips + _random.Next(-2, 3);
            if (sips < 2)
                sips = 1;

            return sips >= min ? sips : min;
        }

        public SlideType SelectNextSlide()
        {
            var counter = 0;
            var handicap = _status.RecentSlides.Select(slide => (slide, _status.RecentSlides.Count - counter++))
                .GroupBy(x => x.slide).ToDictionary(x => x.Key, x => x.Sum(y => y.Item2));
            double max = (_status.RecentSlides.Count * _status.RecentSlides.Count + 1d) / 2d;

            var result = SelectRandomWeighted(ConvertToReadOnly(_status.SlideTypes), value =>
            {
                var weight = value.Weight;
                if (handicap.TryGetValue(value.Value, out var x))
                    weight *= (max - x) / max;

                return weight;
            }).Value;

            _status.RecentSlides.Add(result);
            while (_status.RecentSlides.Count > MaxRecentSlides)
                _status.RecentSlides.RemoveAt(_status.RecentSlides.Count - 1);

            _status.SlidesCounter++;
            return result;
        }

        private static IReadOnlyList<T> ConvertToReadOnly<T>(IList<T> value)
        {
            if (value is IReadOnlyList<T> readOnlyList)
                return readOnlyList;

            return value.ToList();
        }
    }

    public interface ISelectionAlgorithm
    {
        void Initialize(DrinctetStatus status, Random random);

        IReadOnlyList<PlayerInfo> SelectPlayers(IReadOnlyList<RequiredGender> requiredGenders, IReadOnlyList<CardTag> tags);
        int SelectRandomNumber(IReadOnlyList<INumber> numbers);
        string SelectRandomString(IReadOnlyList<string> values);
        TCard SelectCard<TCard>(IReadOnlyList<TCard> cards) where TCard : BaseCard;
        int GetRandomNumber(int minValue, int maxValue);
        int GetRandomNumber(int maxValue);
        bool TrueOrFalse(double trueProbability);
        T SelectRandomWeighted<T>(IReadOnlyList<T> items) where T : IWeighted;
        T SelectRandomWeighted<T>(IReadOnlyList<T> items, Func<T, double> getWeight);
        int GetSips(int min);
        int GetWillPowerLevel();
        SlideType SelectNextSlide();
    }
}