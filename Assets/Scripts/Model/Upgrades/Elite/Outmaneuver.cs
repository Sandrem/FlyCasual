using Ship;
using UnityEngine;
using Upgrade;
using Abilities;
using RuleSets;

namespace UpgradesList
{
    public class Outmaneuver : GenericUpgrade
    {
        public Outmaneuver() : base()
        {
            Types.Add(UpgradeType.Elite);
            Name = "Outmaneuver";
            Cost = 10;

            UpgradeRuleType = typeof(SecondEdition);
        }
    }
}