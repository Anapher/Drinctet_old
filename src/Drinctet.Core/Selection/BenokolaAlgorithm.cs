using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Drinctet.Core.Cards.Base;
#if NETSTANDARD
using Drinctet.Core.Utilities;

#endif

namespace Drinctet.Core.Selection
{
    public class BenokolaAlgorithm : SelectionAlgorithmBase
    {
        public int MaxRecentSlides { get; set; } = 20;
        public bool PairOppositeGenderMoreLikely { get; set; } = true;

        public override IReadOnlyList<PlayerInfo> SelectPlayers(IReadOnlyList<RequiredGender> requiredGenders,
            IReadOnlyList<CardTag> tags)
        {
            if (requiredGenders.Count == 0)
                return Array.Empty<PlayerInfo>();

            if (requiredGenders.Count > Status.Players.Count)
                throw new InvalidCardException();

            int lowestPlayerScore = Status.PlayerScores.Min(x => x.Value);
            int greatestDifference = Status.PlayerScores.Max(x => x.Value) - lowestPlayerScore;

            var pairOppositeGenderMoreLikely = PairOppositeGenderMoreLikely &&
                                               Math.Abs(0.5 - Status.Players.Count(x => x.Gender == Gender.Male) /
                                                        (double) Status.Players.Count) < 0.2;

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
                            sourceList = Status.Players;
                            break;
                        case RequiredGender.Male:
                            sourceList = Status.Players.Where(x => x.Gender == Gender.Male);
                            break;
                        case RequiredGender.Female:
                            sourceList = Status.Players.Where(x => x.Gender == Gender.Female);
                            break;
                        default:
                            continue;
                    }

                    var source = sourceList.Where(x => Array.IndexOf(result, x) == -1).ToList();
                    var males = result.Where(x => x != null).Count(x => x.Gender == Gender.Male);
                    var females = result.Where(x => x != null).Count(x => x.Gender == Gender.Female);

                    var player = SelectRandomWeighted(source, p =>
                    {
                        double value;
                        if (greatestDifference == 0)
                            value = 1;
                        else
                            value = 1 + 0.25 * ((lowestPlayerScore + greatestDifference - Status.PlayerScores[p.Id]) /
                                    (double) greatestDifference);

                        if (forArrangement.Contains(p))
                        {
                            value += Status.Players.Count * 0.25d; // 50/50 that the couple is selected

                            if (tags?.Contains(CardTag.Sexual) == true)
                            {
                                value += Status.Players.Count * 0.25d; // 2/3
                            }
                        }
                        else
                        {
                            //try to pair females / males
                            if (gender == RequiredGender.None && pairOppositeGenderMoreLikely)
                            {
                                if (males > females)
                                {
                                    if (p.Gender == Gender.Female)
                                        value += 0.5;
                                }
                                else if (females > males)
                                {
                                    if (p.Gender == Gender.Male)
                                        value += 0.5;
                                }
                            }
                        }

                        Console.WriteLine(p + " -> " + value);
                        return value;
                    });

                    result[i] = player;
                    resultCounter++;
                    Status.PlayerScores[player.Id]++;

                    if (Status.PlayerArrangements.TryGetValue(player.Id, out var arranging))
                        forArrangement.Add(Status.Players.First(x => x.Id == arranging));
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

        public override int GetWillPowerLevel()
        {
            if (Status.IsWillPowerStatic)
                return Status.WillPower;

            var willPower = Status.WillPower;

            var localTime = DateTimeOffset.UtcNow.Add(Status.TimeZone.BaseUtcOffset);
            if (localTime.Hour > 22 || localTime.Hour < 8)
            {
                if (!Status.WillPowerMemory.ContainsKey("After_10"))
                {
                    willPower++;
                    Status.WillPowerMemory.Add("After_10", null);
                }
            }

            if (localTime.Hour < 8)
            {
                if(!Status.WillPowerMemory.ContainsKey("After_12"))
                {
                    willPower++;
                    Status.WillPowerMemory.Add("After_12", null);
                }
            }

            int hours;
            if (Status.WillPowerMemory.TryGetValue("LastTimeAdded", out var time))
            {
                var lastTimeAdded = DateTimeOffset.ParseExact(time, "O", CultureInfo.InvariantCulture);
                hours = (int) (localTime - lastTimeAdded).TotalHours;
            }
            else
            {
                hours = (int) (localTime - Status.StartTime).TotalHours;
            }

            if (hours >= 1)
            {
                willPower += hours;
                Status.WillPowerMemory["LastTimeAdded"] = Status.StartTime.AddHours(hours)
                    .ToString("O", CultureInfo.InvariantCulture);
            }

            int diff;
            if (Status.WillPowerMemory.TryGetValue("LastSlidesCount", out var slidesCountString))
                diff = Status.SlidesCounter - int.Parse(slidesCountString);
            else diff = Status.SlidesCounter;

            if (diff > 12)
            {
                willPower++;
                Status.WillPowerMemory["LastSlidesCount"] = Status.SlidesCounter.ToString();
            }

            if (willPower > 10)
                willPower = 10;

            Status.WillPower = willPower;

            willPower = (int) (willPower * Status.WillPowerMultiplicator);

            return willPower > 10 ? 10 : willPower;
        }

        //public TCard SelectCard2<TCard>(IReadOnlyList<TCard> cards)
        //{
        //    var willPower = GetWillPowerLevel();

        //    var factor = Math.Max(Status.CardHistory.Count, 10);
        //    var cardsScore = cards.ToDictionary(x => x, x => factor);
        //    var willPowerDistribution = new Dictionary<int, int>();

        //    foreach (var card in cards)
        //    {
        //        if (willPowerDistribution.TryGetValue(card.WillPower, out var count))
        //            willPowerDistribution[card.WillPower] = count + 1;
        //        else willPowerDistribution[card.WillPower] = 1;

        //        if (card is TextCard textCard)
        //        {
        //            if (!textCard.Content.Any(x =>
        //                x.Translations.Any(y => y.Key.TwoLetterISOLanguageName.StartsWith(Status.Language))))
        //                cardsScore[card] -= factor / 2;
        //        }
        //    }

        //    for (int i = Status.CardHistory.Count - 1; i >= 0; i--)
        //    {
        //        var id = Status.CardHistory[i];
        //        var card = cards.FirstOrDefault(x => x.Id == id);
        //        if (card != null)
        //            cardsScore[card] -= i;
        //    }

        //    var result = SelectRandomWeighted(cards, card =>
        //    {
        //        var weight = cardsScore[card];
        //        if (weight < 0)
        //            return 0;

        //        var diff = willPower - card.WillPower;
        //        if (diff < -3) //we dont want to go that far forward
        //            return 0;

        //        if (diff == 0)
        //            weight += factor;

        //        if (diff < 0) //forward
        //            diff = Math.Abs(diff);

        //        weight += factor / (diff + 1);

        //        //if (card.Tags.Any())
        //        //{
        //        //    var min = card.Tags.Select(x => Status.Tags.FirstOrDefault(y => y.Value == x)?.Weight ?? 0).Min();
        //        //    weight *= min;
        //        //}

        //        return (double) weight / willPowerDistribution[card.WillPower];
        //    });

        //    Status.CardHistory.Add(result.Id);
        //    return result;
        //}

        public override TCard SelectCard<TCard>(IReadOnlyList<TCard> cards)
        {
            var willPower = GetWillPowerLevel();
            var totalCards = cards.Count;
            var willPowerDistribution = cards.GroupBy(x => x.WillPower).ToDictionary(x => x.Key, x => x.Count());

            var cardsWeight = cards.Select(card =>
            {
                var weight = willPowerDistribution[card.WillPower] / (double) totalCards;

                if (card.WillPower == willPower)
                    weight *= 2.5;
                else if (card.WillPower >= willPower - 2 && card.WillPower <= willPower + 1)
                    weight *= 2;
                else if (card.WillPower > willPower)
                    weight = weight * (0.3 - (card.WillPower - willPower) * 0.1);
                else //card.WillPower < willPower
                    weight = weight * (0.8 - (willPower - card.WillPower) * 0.1);

                if (weight <= 0)
                    return (card, 0);

                if (card.Tags?.Any() == true)
                {
                    var min = card.Tags.Select(x => Status.Tags.FirstOrDefault(y => y.Value == x)?.Weight ?? 0).Min();
                    weight *= min;
                }

                return (card, weight);
            }).Where(x => x.weight > 0).ToDictionary(x => x.card, x => x.weight);

            var noRepetitionSeries = (int) Math.Ceiling(cardsWeight.Count / 3d * 2d);
            var historicalCards = new HashSet<int>(Status.CardHistory.Where(x => cards.Any(y => y.Id == x)).TakeLast(noRepetitionSeries));

            var result = SelectRandomWeighted(cards, card =>
            {
                if (!historicalCards.Contains(card.Id) && cardsWeight.TryGetValue(card, out var weight))
                    return weight;

                return 0;
            });

            if (result == null)
                throw new InvalidCardException();

            Console.WriteLine($"[Current WillPower: {willPower}; picked card: {result.WillPower}]");
            Status.CardHistory.Add(result.Id);
            return result;
        }

        public override int GetSips(int min)
        {
            var baseSips = (int) (Status.DrinkALot * 4);

            var timespan = DateTimeOffset.UtcNow - Status.StartTime;

            if ((int) Math.Floor(timespan.TotalHours) % 2 == 0)
                baseSips += 1;

            var localTime = DateTimeOffset.UtcNow.Add(Status.TimeZone.BaseUtcOffset);
            if (localTime.Hour < 22 && localTime.Hour > 12)
                baseSips += 1;

            var sips = baseSips + Random.Next(-2, 3);
            if (sips < 2)
                sips = 1;

            return sips >= min ? sips : min;
        }

        public override SlideType SelectNextSlide()
        {
            var counter = 0;
            var handicap = Status.RecentSlides.Select(slide => (slide, Status.RecentSlides.Count - counter++))
                .GroupBy(x => x.slide).ToDictionary(x => x.Key, x => x.Sum(y => y.Item2));
            double max = (Status.RecentSlides.Count * Status.RecentSlides.Count + 1d) / 2d;

            var result = SelectRandomWeighted(ConvertToReadOnly(Status.SlideTypes), value =>
            {
                var weight = value.Weight;
                if (handicap.TryGetValue(value.Value, out var x))
                    weight *= (max - x) / max;

                return weight;
            }).Value;

            Status.RecentSlides.Add(result);
            while (Status.RecentSlides.Count > MaxRecentSlides)
                Status.RecentSlides.RemoveAt(Status.RecentSlides.Count - 1);

            Status.SlidesCounter++;
            return result;
        }

        private static IReadOnlyList<T> ConvertToReadOnly<T>(IList<T> value)
        {
            if (value is IReadOnlyList<T> readOnlyList)
                return readOnlyList;

            return value.ToList();
        }
    }
}