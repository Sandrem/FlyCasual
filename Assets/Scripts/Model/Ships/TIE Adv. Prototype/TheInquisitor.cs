using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ship;

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

                PilotAbilities.Add(new PilotAbilitiesNamespace.TheInquisitorAbility());
            }
        }
    }
}

namespace PilotAbilitiesNamespace
{
    public class TheInquisitorAbility : GenericPilotAbility
    {
        public override void Initialize(GenericShip host)
        {
            base.Initialize(host);

            Board.ShipShotDistanceInformation.OnRangeIsMeasured += SetRangeToOne;
        }

        private void SetRangeToOne(GenericShip thisShip, GenericShip anotherShip, ref int range)
        {
            if (thisShip.ShipId == Host.ShipId) range = 1;
        }
    }
}
