using System;
using System.Collections.Generic;
using System.Linq;
using Drinctet.Core.Cards.Base;

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
                            value = 1 + 0.25 * ((Status.PlayerScores[p.Id] - lowestPlayerScore) /
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
            var willPower = 0;
            if (Status.StaticWillPower == null)
            {
                var localTime = DateTimeOffset.UtcNow.Add(Status.TimeZone.BaseUtcOffset);
                if (localTime.Hour > 22 || localTime.Hour < 8)
                    willPower++;

                if (localTime.Hour < 8)
                    willPower++;

                willPower += (int)(localTime - Status.StartTime).TotalHours;
                willPower += Status.SlidesCounter / 12;
                willPower = (int)(willPower * Status.WillPowerMultiplicator);
            }
            else willPower = Status.StaticWillPower.Value;

            if (willPower > 10)
                willPower = 10;

            return willPower;
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

            var noRepetitionSeries = cardsWeight.Count / 3 * 2;
            var historicalCards = new HashSet<int>(Status.CardHistory.Where(x => cards.Any(y => y.Id == x)).Take(noRepetitionSeries));

            var result = SelectRandomWeighted(cards, card =>
            {
                if (!historicalCards.Contains(card.Id) && cardsWeight.TryGetValue(card, out var weight))
                    return weight;

                return 0;
            });

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