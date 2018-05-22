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
    public class Selfless : GenericUpgrade
    {
        public Selfless() : base()
        {
            Types.Add(UpgradeType.Elite);
            Name = "Selfless";
            Cost = 1;

            UpgradeRuleType = typeof(SecondEdition);
        }
    }
}
