using System.Collections;
using System.Collections.Generic;
using Upgrade;
using Abilities;

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

            UpgradeAbilities.Add(new DorsalTurretAbility());
        }        
    }
}

namespace Abilities
{
    public class DorsalTurretAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.AfterGotNumberOfAttackDice += AddDiceAtRangeOne;
        }

        public override void DeactivateAbility()
        {
            HostShip.AfterGotNumberOfAttackDice -= AddDiceAtRangeOne;
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
