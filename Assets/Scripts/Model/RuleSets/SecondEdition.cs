using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RuleSets
{
    public class SecondEdition : RuleSet
    {
        public override string Name { get { return "Second Edition"; } }

        public override int MaxPoints { get { return 200; } }
        public override int MinShipsCount { get { return 1; } }
        public override int MaxShipsCount { get { return 8; } }
    }
}
