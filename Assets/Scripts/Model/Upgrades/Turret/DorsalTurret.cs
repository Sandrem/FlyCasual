using System.Collections;
using System.Collections.Generic;
using Upgrade;

namespace UpgradesList
{

    public class DorsalTurret : GenericSecondaryWeapon
    {
        public DorsalTurret() : base()
        {
            Types.Add(UpgradeType.Turret);

            Name = "Dorsal Turret";
            Cost = 3;

            MinRange = 1;
            MaxRange = 2;
            AttackValue = 2;

            CanShootOutsideArc = true;
        }

        public override void AttachToShip(Ship.GenericShip host)
        {
            base.AttachToShip(host);

            Host.AfterGotNumberOfAttackDice += AddDiceAtRangeOne;
        }

        private void AddDiceAtRangeOne(ref int diceCount)
        {
            if (Combat.ChosenWeapon == this && Combat.ShotInfo.Range == 1)
            {
                diceCount++;
            }
        }
    }
}
