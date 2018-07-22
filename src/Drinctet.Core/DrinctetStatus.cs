using System.Collections.Generic;
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
        public IDictionary<int, int> PlayerArrangements { get; set; }

        /// <summary>
        ///     The tags with their weight. All tags that are not present in this list have a default weight of 0.5 (including
        ///     cards with no tags)
        /// </summary>
        public IList<WeightedValue<CardTag>> Tags { get; set; }

        /// <summary>
        ///     The slides that will be displayed with their weight. All slides that are not present in this list will never be
        ///     shown up
        /// </summary>
        public IList<WeightedValue<SlideType>> SlideTypes { get; set; }

        /// <summary>
        ///     A value between 0 to 1 that indicates how much the persons should drink. The value 1 means that a lot of sips will
        ///     be distributed
        /// </summary>
        public double DrinkALot { get; set; }

        /// <summary>
        ///     The score of each player that indicates how many cards the played and how much they had to drink. The key value is
        ///     the player id
        /// </summary>
        public IDictionary<int, int> PlayerScores { get; set; }

        /// <summary>
        ///     All cards that were played in this game
        /// </summary>
        public IList<int> CardHistory { get; set; }

        /// <summary>
        ///     The 20 recent slides
        /// </summary>
        public IList<SlideType> RecentSlides { get; set; }

        /// <summary>
        ///     The selection algorithm that is used to select cards, players, slides, sips and will power
        /// </summary>
        public SelectionAlgorithm SelectionAlgorithm { get; set; }
    }

    public enum SelectionAlgorithm
    {
        Benokla
    }

    public enum SlideType
    {
        Balancing,
        BlackOrRed,
        QuizTrust,
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