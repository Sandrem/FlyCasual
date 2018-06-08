using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;
using Abilities;
using RuleSets;
using Ship;
using System;

namespace UpgradesList
{

    public class R4Astromech : GenericUpgrade
    {

        public R4Astromech() : base()
        {
            Types.Add(UpgradeType.Astromech);
            Name = "R4 Astromech";
            Cost = 10;

            UpgradeRuleType = typeof(SecondEdition);
        }
    }

}
