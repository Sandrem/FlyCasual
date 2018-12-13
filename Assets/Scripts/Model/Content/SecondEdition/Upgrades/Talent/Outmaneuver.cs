﻿using Upgrade;
using System.Collections.Generic;
using Ship;

namespace UpgradesList.SecondEdition
{
    public class Outmaneuver : GenericUpgrade
    {
        public Outmaneuver() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Outmaneuver",
                UpgradeType.Talent,
                cost: 6,
                abilityType: typeof(Abilities.FirstEdition.OutmaneuverAbility),
                seImageNumber: 11
            );
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class OutmaneuverAbilitySE : Abilities.FirstEdition.OutmaneuverAbility
    {
        protected override bool AbilityCanBeUsed()
        {
            if (!Combat.ShotInfo.InPrimaryArc) return false;

            BoardTools.ShotInfo reverseShotInfo = new BoardTools.ShotInfo(Combat.Defender, Combat.Attacker, Combat.Defender.PrimaryWeapons);
            if (reverseShotInfo.InArc) return false;

            return true;
        }
    }
}