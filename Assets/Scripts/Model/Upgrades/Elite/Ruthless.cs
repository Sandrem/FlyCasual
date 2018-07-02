using Upgrade;
using UnityEngine;
using Ship;
using System.Collections.Generic;
using SubPhases;
using System;
using Abilities;
using RuleSets;

namespace UpgradesList
{
    public class Elusive : GenericUpgrade
    {
        public Elusive() : base()
        {
            Types.Add(UpgradeType.Elite);
            Name = "Elusive";
            Cost = 1;

            UpgradeRuleType = typeof(SecondEdition);

            ImageUrl = "https://i.imgur.com/D0w1eiJ.png";
        }
    }
}
