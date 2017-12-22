using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Abilities;

namespace Ship
{
    namespace TIEFighter
    {
        public class SabineWren : TIEFighter
        {
            public SabineWren() : base()
            {
                PilotName = "Sabine Wren";
                PilotSkill = 5;
                Cost = 15;

                faction = Faction.Rebel;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                PilotAbilities.Add(new SabineWrenPilotAbility());
            }
        }
    }
}
