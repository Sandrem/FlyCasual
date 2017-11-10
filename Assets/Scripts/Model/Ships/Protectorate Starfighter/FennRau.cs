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
                ImageUrl = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/pilots/Scum%20and%20Villainy/Protectorate%20Starfighter/fenn-rau.png";
                PilotSkill = 9;
                Cost = 28;

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
