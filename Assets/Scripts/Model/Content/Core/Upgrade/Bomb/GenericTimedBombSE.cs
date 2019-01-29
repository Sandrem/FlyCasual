using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Upgrade
{
    abstract public class GenericTimedBombSE : GenericTimedBomb
    {
        public GenericTimedBombSE()
        {
            this.IsDiscardedAfterDropped = false;
        }
    }
}
