using System;
using ActionsList;
using Ship;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class MunitionsFailsafe : GenericUpgrade
    {
        public MunitionsFailsafe() : base()
        {
            UpgradeInfo = new UpgradeCardInfo("Munitions Failsafe",
                UpgradeType.Modification,
                cost: 2,
                abilityType: typeof(Abilities.SecondEdition.MunitionsFailsafeAbility),
                seImageNumber: 74);
        }
    }
}

namespace Abilities.SecondEdition
{
    // While you perform a [torpedo] or [missile] attack, after rolling 
    // attack dice, you may cancel all dice results to recover 1 [charge] you
    // spent as a cost for the attack.
    public class MunitionsFailsafeAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            GenericShip.OnGenerateDiceModificationsGlobal += AddMunitionsFailsafeAbility;

        }

        public override void DeactivateAbility()
        {
            GenericShip.OnGenerateDiceModificationsGlobal -= AddMunitionsFailsafeAbility;

        }

        private void AddMunitionsFailsafeAbility(GenericShip ship)
        {
            Combat.Attacker.AddAvailableDiceModification(new MunitionsFailsafeAction() { HostShip = this.HostShip });
        }

        private class MunitionsFailsafeAction : GenericAction
        {
            public MunitionsFailsafeAction()
            {
                Name = DiceModificationName = "Munitions Failsafe: Cancel all results";
             }
            public override bool IsDiceModificationAvailable()
            {
                if (Combat.ChosenWeapon.WeaponType == Ship.WeaponTypes.Missile || Combat.ChosenWeapon.WeaponType == Ship.WeaponTypes.Torpedo)
                {
                    return true;
                }

                return false;
            }

            public override void ActionEffect(Action callBack)
            {
                Combat.DiceRollAttack.CancelAllResults();
                var weapon = Combat.ChosenWeapon as GenericSpecialWeapon;
                if (weapon != null) {
                    weapon.State.RestoreCharge();
                }
                callBack();
            }

        }
    }
}
