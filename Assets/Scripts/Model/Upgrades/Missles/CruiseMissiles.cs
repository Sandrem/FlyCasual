using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Upgrade;

namespace UpgradesList
{

    public class CruiseMissiles : GenericSecondaryWeapon
    {
        public CruiseMissiles() : base()
        {
            Type = UpgradeType.Missile;

            Name = "Cruise Missiles";
            Cost = 3;

            MinRange = 2;
            MaxRange = 3;
            AttackValue = 1;

            RequiresTargetLockOnTargetToShoot = true;

            IsDiscardedForShot = true;
        }

        public override void AttachToShip(Ship.GenericShip host)
        {
            base.AttachToShip(host);

            Host.AfterGotNumberOfAttackDice += CruiseMissilesAbility;
        }

        private void CruiseMissilesAbility(ref int diceCount)
        {
            if (Combat.ChosenWeapon == this)
            {
                if (Combat.Attacker.AssignedManeuver != null) diceCount += Mathf.Min(Combat.Attacker.AssignedManeuver.Speed, 4);
            }
        }

    }

}