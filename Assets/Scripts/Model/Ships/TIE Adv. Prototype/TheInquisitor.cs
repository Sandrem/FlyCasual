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
                ImageUrl = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/pilots/Galactic%20Empire/TIE%20Adv.%20Prototype/the-inquisitor.png";
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
