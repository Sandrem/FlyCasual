using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ship;
using System.Linq;
using Tokens;
using Abilities;

namespace Ship
{
    namespace AuzituckGunship
    {
        public class Wullffwarro : AuzituckGunship
        {
            public Wullffwarro() : base()
            {
                PilotName = "Wullffwarro";
                PilotSkill = 7;
                Cost = 30;

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                PilotAbilities.Add(new WullffwarroAbility());
            }
        }
    }
}

namespace Abilities
{
    public class WullffwarroAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.AfterGotNumberOfAttackDice += CheckWullffwarroAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.AfterGotNumberOfAttackDice -= CheckWullffwarroAbility;
        }

        private void CheckWullffwarroAbility(ref int value)
        {
            if ((HostShip.Shields == 0) && (HostShip.Hull < HostShip.MaxHull)) value++;
        }
    }
}
