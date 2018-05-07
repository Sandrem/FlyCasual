using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RuleSets
{
    public class Epic : FirstEdition
    {
        public override string Name { get { return "Epic"; } }

        public override int MaxPoints { get { return 300; } }
    }
}
