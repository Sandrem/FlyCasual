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
    public class Ruthless : GenericUpgrade
    {
        public Ruthless() : base()
        {
            Types.Add(UpgradeType.Elite);
            Name = "Ruthless";
            Cost = 1;

            UpgradeRuleType = typeof(SecondEdition);

            ImageUrl = "https://i.imgur.com/0ID4yWs.png";
        }
    }
}
