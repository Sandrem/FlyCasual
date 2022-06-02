using Upgrade;
using Ship;
using Arcs;
using System.Linq;
using UnityEngine;

namespace UpgradesList.SecondEdition
{
    public class VeteranTailGunner : GenericUpgrade
    {
        public VeteranTailGunner() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Veteran Tail Gunner",
                UpgradeType.Gunner,
                cost: 3,
                abilityType: typeof(Abilities.SecondEdition.VeteranTailGunnerAbility),
                restriction: new ArcRestriction(ArcType.Rear),
                seImageNumber: 51
            );

            Avatar = new AvatarInfo(
                Faction.Rebel,
                new Vector2(409, 32),
                new Vector2(125, 125)
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

                Combat.StartSelectAttackTarget(
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
                Messages.ShowErrorToHuman(string.Format("{0} cannot make additional attacks", HostShip.PilotInfo.PilotName));
                Triggers.FinishTrigger();
            }
        }

        private void FinishAdditionalAttack()
        {
            // If attack is skipped, set this flag, otherwise regular attack can be performed second time
            HostShip.IsAttackPerformed = true;

            //if bonus attack was skipped, allow bonus attacks again
            if (HostShip.IsAttackSkipped) HostShip.IsCannotAttackSecondTime = false;

            Triggers.FinishTrigger();
        }

        private bool IsRearArcShot(GenericShip defender, IShipWeapon weapon, bool isSilent)
        {
            bool result = false;

            if (weapon.WeaponType == WeaponTypes.PrimaryWeapon && weapon.WeaponInfo.ArcRestrictions.Contains(ArcType.Rear))
            {
                result = true;
            }
            else
            {
                if (!isSilent) Messages.ShowError("This attack must use the ship's rear firing arc");
            }

            return result;
        }
    }
}