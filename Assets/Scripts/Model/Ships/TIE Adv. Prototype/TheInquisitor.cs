﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ship;
using System;
using BoardTools;

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
            ShotInfo.OnRangeIsMeasured += SetRangeToOne;
        }

        public override void DeactivateAbility()
        {
            ShotInfo.OnRangeIsMeasured -= SetRangeToOne;
        }

        private void SetRangeToOne(GenericShip thisShip, GenericShip anotherShip, IShipWeapon chosenWeapon, ref int range)
        {
            if (thisShip.ShipId == HostShip.ShipId)
            {
                if ((range <= 3) && (chosenWeapon.GetType() == typeof(PrimaryWeaponClass)))
                {
                    range = 1;
                }
            }
        }
    }
}
