using System;
using System.Collections.Generic;
using System.Linq;
using Drinctet.Core.Cards.Base;

namespace Drinctet.Core
{
    public class DrinctetStatus
    {
        public IList<PlayerInfo> Players { get; set; } = new List<PlayerInfo>();
        public string Language { get; set; } = "en";

        /// <summary>
        ///     Players that should be paired more often than others. The value is the id
        /// </summary>
        public IDictionary<int, int> PlayerArrangements { get; set; } = new Dictionary<int, int>();

        /// <summary>
        ///     The tags with their weight. All tags that are not present in this list have a default weight of 0.5 (including
        ///     cards with no tags)
        /// </summary>
        public IList<WeightedValue<CardTag>> Tags { get; set; } = Enum.GetValues(typeof(CardTag)).Cast<CardTag>()
            .Select(x => new WeightedValue<CardTag>(x, 1)).ToList();

        /// <summary>
        ///     The slides that will be displayed with their weight. All slides that are not present in this list will never be
        ///     shown up
        /// </summary>
        public IList<WeightedValue<SlideType>> SlideTypes { get; set; } = new List<WeightedValue<SlideType>>
        {
            new WeightedValue<SlideType>(SlideType.Down, .05),
            new WeightedValue<SlideType>(SlideType.Drink, .5),
            new WeightedValue<SlideType>(SlideType.GroupGame, .3),
            new WeightedValue<SlideType>(SlideType.NeverEver, .4),
            new WeightedValue<SlideType>(SlideType.Question, .5),
            new WeightedValue<SlideType>(SlideType.Virus, .5),
            new WeightedValue<SlideType>(SlideType.Task, .5),
            new WeightedValue<SlideType>(SlideType.WouldYouRather, .4),
            new WeightedValue<SlideType>(SlideType.SocialMedia, .1),
            new WeightedValue<SlideType>(SlideType.NoIdeaLoses, .2),
        };

        /// <summary>
        ///     A value between 0 to 1 that indicates how much the persons should drink. The value 1 means that a lot of sips will
        ///     be distributed
        /// </summary>
        public double DrinkALot { get; set; } = 0.5;

        /// <summary>
        ///     The score of each player that indicates how many cards the played and how much they had to drink. The key value is
        ///     the player id
        /// </summary>
        public IDictionary<int, int> PlayerScores { get; set; } = new Dictionary<int, int>();

        /// <summary>
        ///     All cards that were played in this game
        /// </summary>
        public IList<int> CardHistory { get; set; } = new List<int>();

        /// <summary>
        ///     The 20 recent slides
        /// </summary>
        public IList<SlideType> RecentSlides { get; set; } = new List<SlideType>();

        /// <summary>
        ///     The selection algorithm that is used to select cards, players, slides, sips and will power
        /// </summary>
        public SelectionAlgorithm SelectionAlgorithm { get; set; } = SelectionAlgorithm.Benokla;

        /// <summary>
        ///     The amount of slides that were displayed
        /// </summary>
        public int SlidesCounter { get; set; } = 0;

        /// <summary>
        ///     The local time zone
        /// </summary>
        public TimeZoneInfo TimeZone { get; set; } = TimeZoneInfo.Local;

        /// <summary>
        ///     The start time
        /// </summary>
        public DateTimeOffset StartTime { get; set; } = DateTimeOffset.UtcNow;

        /// <summary>
        ///     A static will power value that will not change
        /// </summary>
        public bool IsWillPowerStatic { get; set; }

        /// <summary>
        ///     A multiplicator for will power
        /// </summary>
        public double WillPowerMultiplicator { get; set; } = 1;

        /// <summary>
        ///     The used social media platform (Snapchat, Facebook, Instagram, WhatsApp)
        /// </summary>
        public string SocialMediaPlatform { get; set; } = "Snapchat";

        /// <summary>
        ///     The current will power level
        /// </summary>
        public int WillPower { get; set; }

        /// <summary>
        ///     Data needed by the algorithm to increase the will power linear
        /// </summary>
        public IDictionary<string, string> WillPowerMemory { get; set; } = new Dictionary<string, string>();

        public void UpdatePlayers()
        {
            foreach (var playerInfo in Players)
            {
                if (playerInfo.Id == 0)
                {
                    playerInfo.Id = Players.Select(x => x.Id).Max() + 1;
                }
            }

            var medianScore = !PlayerScores.Any() ? 0 : PlayerScores.Values.Aggregate(0, (x, y) => x + y) / PlayerScores.Count;

            var removedPlayers = PlayerScores.Keys.ToList();
            foreach (var playerInfo in Players)
            {
                if (!PlayerScores.ContainsKey(playerInfo.Id))
                    PlayerScores.Add(playerInfo.Id, medianScore);
                else
                    removedPlayers.Remove(playerInfo.Id);
            }

            foreach (var removedPlayer in removedPlayers)
            {
                PlayerScores.Remove(removedPlayer);
            }
        }
    }

    public enum SelectionAlgorithm
    {
        Benokla
    }

    public enum SlideType
    {
        //Balancing,
        //BlackOrRed,
        //QuizTrust,
        Down,
        Drink,
        GroupGame,
        NeverEver,
        NoIdeaLoses,
        Question,
        SocialMedia,
        Task,
        Virus,
        WouldYouRather
    }
}