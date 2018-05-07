using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RuleSets
{
    public class SingleShip32pts : FirstEdition
    {
        public override string Name { get { return "Single Ship 32pts"; } }

        public override int MaxPoints { get { return 32; } }
        public override int MaxShipsCount { get { return 1; } }
    }
}
