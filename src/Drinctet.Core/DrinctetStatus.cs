using System.Collections.Generic;

namespace Drinctet.Core
{
    public class DrinctetStatus
    {
        public IList<PlayerInfo> Players { get; set; } = new List<PlayerInfo>();
        public string Language { get; set; } = "en";
    }
}