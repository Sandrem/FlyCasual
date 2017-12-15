using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ship;
using System;

namespace Ship
{
    namespace TIEAdvPrototype
    {
        public class TheInquisitor : TIEAdvPrototype
        {
            public TheInquisitor() : base()
            {
                PilotName = "The Inquisitor";
                PilotSkill = 8;
                Cost = 25;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                PilotAbilities.Add(new Abilities.TheInquisitorAbility());
            }
        }
    }
}

namespace Abilities
{
    public class TheInquisitorAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            Board.ShipShotDistanceInformation.OnRangeIsMeasured += SetRangeToOne;
        }

        public override void DeactivateAbility()
        {
            Board.ShipShotDistanceInformation.OnRangeIsMeasured -= SetRangeToOne;
        }

        private void SetRangeToOne(GenericShip thisShip, GenericShip anotherShip, ref int range)
        {
            if (thisShip.ShipId == HostShip.ShipId) range = 1;
        }
    }
}
