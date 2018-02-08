using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace ProtectorateStarfighter
    {
        public class FennRau : ProtectorateStarfighter
        {
            public FennRau() : base()
            {
                PilotName = "Fenn Rau";
                PilotSkill = 9;
                Cost = 28;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                PilotAbilities.Add(new Abilities.FennRauScumAbility());
            }
        }
    }
}

namespace Abilities
{
    public class FennRauScumAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.AfterGotNumberOfAttackDice += CheckFennRauAbility;
            HostShip.AfterGotNumberOfDefenceDice += CheckFennRauAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.AfterGotNumberOfAttackDice -= CheckFennRauAbility;
            HostShip.AfterGotNumberOfDefenceDice -= CheckFennRauAbility;
        }

        private void CheckFennRauAbility(ref int value)
        {
            if (Combat.ShotInfo.Range == 1) value++;
        }
    }
}
