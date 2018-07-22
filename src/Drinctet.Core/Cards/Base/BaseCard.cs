using System.Collections.Generic;

namespace Drinctet.Core.Cards.Base
{
    public abstract class BaseCard
    {
        /// <summary>
        ///     The id of the card
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        ///     The will power that is needed to execute the card. The will power must be an integer from 1 to 10.
        /// </summary>
        public virtual int WillPower { get; internal set; }

        /// <summary>
        ///     The condition that must match to execute this card
        /// </summary>
        public virtual object Condition { get; internal set; }

        /// <summary>
        ///     The configuration of the players
        /// </summary>
        public virtual IReadOnlyList<PlayerSettings> Players { get; internal set; }

        /// <summary>
        ///     The source of this card if applicable
        /// </summary>
        public virtual string Source { get; internal set; }

        /// <summary>
        ///     Special tags of this card
        /// </summary>
        public virtual IReadOnlyList<CardTag> Tags { get; internal set; }
    }
}