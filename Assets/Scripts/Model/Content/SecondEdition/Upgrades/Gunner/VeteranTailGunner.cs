using Upgrade;
using Ship;
using Arcs;
using System.Linq;

namespace UpgradesList.SecondEdition
{
    public class VeteranTailGunner : GenericUpgrade
    {
        public VeteranTailGunner() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Veteran Tail Gunner",
                UpgradeType.Gunner,
                cost: 4,
                abilityType: typeof(Abilities.SecondEdition.VeteranTailGunnerAbility),
                restriction: new ArcRestriction(ArcType.Rear),
                seImageNumber: 51
            );
        }
    }
}

namespace Abilities.SecondEdition
{
    public class VeteranTailGunnerAbility : GenericAbility
    {
        // After you perform a primary forward firing arc attack, 
        // you may perform a bonus primary rear firing arc attack.

        public override void ActivateAbility()
        {
            HostShip.OnAttackFinishAsAttacker += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackFinishAsAttacker -= CheckAbility;
        }

        private void CheckAbility(GenericShip ship)
        {
            if (Combat.ShotInfo.Weapon.WeaponType != WeaponTypes.PrimaryWeapon) return;
            if (Combat.ArcForShot.ArcType != ArcType.Front) return;

            HostShip.OnCombatCheckExtraAttack += RegisterSecondAttackTrigger;
        }

        private void RegisterSecondAttackTrigger(GenericShip ship)
        {
            HostShip.OnCombatCheckExtraAttack -= RegisterSecondAttackTrigger;

            RegisterAbilityTrigger(TriggerTypes.OnCombatCheckExtraAttack, UseGunnerAbility);
        }

        private void UseGunnerAbility(object sender, System.EventArgs e)
        {
            if (!HostShip.IsCannotAttackSecondTime)
            {
                HostShip.IsCannotAttackSecondTime = true;

                Combat.StartAdditionalAttack(
                    HostShip,
                    FinishAdditionalAttack,
                    IsRearArcShot,
                    HostUpgrade.UpgradeInfo.Name,
                    "You may perform a bonus primary rear firing arc attack",
                    HostUpgrade
                );
            }
            else
            {
                Messages.ShowErrorToHuman(string.Format("{0} cannot attack one more time", HostShip.PilotInfo.PilotName));
                Triggers.FinishTrigger();
            }
        }

        private void FinishAdditionalAttack()
        {
            // If attack is skipped, set this flag, otherwise regular attack can be performed second time
            HostShip.IsAttackPerformed = true;

            Triggers.FinishTrigger();
        }

        private bool IsRearArcShot(GenericShip defender, IShipWeapon weapon, bool isSilent)
        {
            bool result = false;

            if (Combat.ChosenWeapon.WeaponType == WeaponTypes.PrimaryWeapon && Combat.ChosenWeapon.WeaponInfo.ArcRestrictions.Contains(ArcType.Rear))
            {
                result = true;
            }
            else
            {
                if (!isSilent) Messages.ShowError("Attack must use rear firing arc");
            }

            return result;
        }
    }
}