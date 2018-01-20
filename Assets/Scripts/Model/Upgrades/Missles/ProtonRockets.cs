using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ship;
using UnityEngine;
using Upgrade;

namespace UpgradesList
{
    public class ProtonRockets : GenericSecondaryWeapon
    {
        public ProtonRockets() : base()
        {
            Type = UpgradeType.Missile;

            Name = "Proton Rockets";
            Cost = 3;

            MinRange = 1;
            MaxRange = 1;
            AttackValue = 2;

            RequiresFocusToShoot = true;

            IsDiscardedForShot = true;
        }

        public override void AttachToShip(GenericShip host)
        {
            base.AttachToShip(host);

            Host.AfterGotNumberOfAttackDice += ProtonRocketsAbility;
        }

        private void ProtonRocketsAbility(ref int diceCount)
        {
            if (Combat.ChosenWeapon == this)
            {
                diceCount += Mathf.Min(Combat.Attacker.Agility, 3);
            }
        }
    }
}