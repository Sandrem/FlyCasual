using Ship;
using Upgrade;
using ActionsList;
using System.Linq;
using UnityEngine;
using SquadBuilderNS;
using Abilities;
using RuleSets;

namespace UpgradesList
{
    public class AfterBurners : GenericUpgrade, ISecondEditionUpgrade
    {
        public AfterBurners() : base()
        {
            Types.Add(UpgradeType.Modification);
            Name = "AfterBurners";
            Cost = 8;

            ImageUrl = "https://i.imgur.com/3ymspED.png";

            UpgradeRuleType = typeof(SecondEdition);
            MaxCharges = 2;
        }

        public void AdaptUpgradeToSecondEdition()
        {
            // No Adaptation is required
        }
    }
}

