using System;
using System.Collections.Generic;
using System.Text;

namespace Drinctet.Core
{
    public class GameManager
    {
        public DrinctetStatus Status { get; }

        public GameManager(DrinctetStatus status)
        {
            Status = status;
        }
    }
}
