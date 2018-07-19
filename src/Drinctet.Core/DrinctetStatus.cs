using System.Collections.Generic;

namespace Drinctet.Core
{
    public class DrinctetStatus
    {
        public DrinctetStatus()
        {
            Players = new List<PlayerInfo>();
        }

        public IList<PlayerInfo> Players { get; }
    }
}