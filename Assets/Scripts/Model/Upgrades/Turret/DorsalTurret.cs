using System.Collections;
using System.Collections.Generic;
using Upgrade;
using Abilities;
using RuleSets;
using ActionsList;
using Arcs;

namespace UpgradesList
{
    public class DorsalTurret : GenericSecondaryWeapon, ISecondEditionUpgrade
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

        public void AdaptUpgradeToSecondEdition()
        {
            Cost = 4;
            CanShootOutsideArc = false;

            ArcRestrictions.Add(Arcs.ArcTypes.Mobile);

            UpgradeAbilities.RemoveAll(a => a is Abilities.DorsalTurretAbility);
            UpgradeAbilities.Add(new Abilities.SecondEdition.DorsalTurretAbility());
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
            if (Combat.ChosenWeapon == HostUpgrade && Combat.ShotInfo.Range == 1)
            {
                diceCount++;
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class DorsalTurretAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.ArcInfo.GetArc<ArcMobile>().ShotPermissions.CanShootPrimaryWeapon = false;
            HostShip.ActionBar.AddGrantedAction(new RotateArcAction(), HostUpgrade);
        }

        public override void DeactivateAbility()
        {
            HostShip.ActionBar.RemoveGrantedAction(typeof(RotateArcAction), HostUpgrade);
        }

    }
}
