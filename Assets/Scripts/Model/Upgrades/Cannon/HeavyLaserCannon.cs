using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;

namespace UpgradesList
{

    public class HeavyLaserCannon : GenericSecondaryWeapon
    {
		public HeavyLaserCannon() : base()
        {
            Type = UpgradeType.Cannon;

            Name = "Heavy Laser Cannon";
            ShortName = "HLC";
            Cost = 7;

            MinRange = 2;
            MaxRange = 3;
            AttackValue = 4;

        }

        public override void AttachToShip(Ship.GenericShip host)
        {
            base.AttachToShip(host);

            Host.OnImmediatelyAfterRolling += ChangeCritsToHits;
        }

        private void ChangeCritsToHits(DiceRoll diceroll)
        {
            if (Combat.ChosenWeapon == this)
            {
                diceroll.ChangeAll(DiceSide.Crit, DiceSide.Success);
            }
        }

    }

}
