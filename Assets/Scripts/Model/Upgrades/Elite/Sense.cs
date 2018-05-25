using Ship;
using UnityEngine;
using Upgrade;
using Abilities;
using RuleSets;

namespace UpgradesList
{
    public class Sense : GenericUpgrade
    {
        public Sense() : base()
        {
            Types.Add(UpgradeType.Elite);
            Name = "Sense";
            Cost = 10;

            ImageUrl = "https://i.imgur.com/WEduTk1.png";

            UpgradeRuleType = typeof(SecondEdition);
        }
    }
}