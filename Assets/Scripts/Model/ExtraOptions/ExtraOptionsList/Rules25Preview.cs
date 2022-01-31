using Editions;
using Editions.RuleSets;
using UnityEngine;

namespace ExtraOptions
{
    namespace ExtraOptionsList
    {
        public class Rules25PreviewExtraOption : ExtraOption
        {
            public Rules25PreviewExtraOption()
            {
                Name = "Preview of rules v2.5";
                Description = "Random player order after dials.\n" +
                    "New rules for attacks at range 0.\n" +
                    "New rules for ship-overlapping.\n" +
                    "New rules for obstacles.";
            }

            protected override void Activate()
            {
                Edition.Current.RuleSet = new RuleSet25();
            }

            protected override void Deactivate()
            {
                Edition.Current.RuleSet = new RuleSet20();
            }
        }
    }
}
