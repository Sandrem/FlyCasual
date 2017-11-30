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

                PilotAbilities.Add(new PilotAbilitiesNamespace.FennRauAbility());
            }
        }
    }
}

namespace PilotAbilitiesNamespace
{
    public class FennRauAbility : GenericPilotAbility
    {
        public override void Initialize(Ship.GenericShip host)
        {
            base.Initialize(host);

            Host.AfterGotNumberOfAttackDice += CheckFennRauAbility;
            Host.AfterGotNumberOfDefenceDice += CheckFennRauAbility;
        }

        private void CheckFennRauAbility(ref int value)
        {
            if (Combat.ShotInfo.Range == 1) value++;
        }
    }
}
