using Upgrade;
using Ship;
using Arcs;
using System.Linq;
using BoardTools;
using System.Collections.Generic;
using UnityEngine;

namespace UpgradesList.SecondEdition
{
    public class BistanGunner : GenericUpgrade
    {
        public BistanGunner() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Bistan",
                UpgradeType.Gunner,
                cost: 8,
                isLimited: true,
                abilityType: typeof(Abilities.SecondEdition.BistanGunnerAbility),
                restriction: new FactionRestriction(Faction.Rebel),
                seImageNumber: 95
            );

            Avatar = new AvatarInfo(
                Faction.Rebel,
                new Vector2(397, 4),
                new Vector2(125, 125)
            );
        }
    }
}

namespace Abilities.SecondEdition
{
    public class BistanGunnerAbility : GenericAbility
    {
        private GenericShip PreviousTarget;

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

            PreviousTarget = Combat.Defender;
            HostShip.OnCombatCheckExtraAttack += TryRegisterSecondAttackTrigger;
        }

        private void TryRegisterSecondAttackTrigger(GenericShip ship)
        {
            HostShip.OnCombatCheckExtraAttack -= TryRegisterSecondAttackTrigger;

            if (HostShip.Tokens.HasToken<Tokens.FocusToken>())
            {
                RegisterAbilityTrigger(TriggerTypes.OnCombatCheckExtraAttack, UseGunnerAbility);
            }
        }

        private void UseGunnerAbility(object sender, System.EventArgs e)
        {
            if (!HostShip.IsCannotAttackSecondTime)
            {
                HostShip.IsCannotAttackSecondTime = true;

                Combat.StartSelectAttackTarget(
                    HostShip,
                    FinishAdditionalAttack,
                    IsTurretAndAnotherTarget,
                    HostUpgrade.UpgradeInfo.Name,
                    "You may perform a bonus turret arc attack against a ship you have not already attacked this round",
                    HostUpgrade
                );
            }
            else
            {
                Messages.ShowErrorToHuman(string.Format("{0} cannot attack an additional time", HostShip.PilotInfo.PilotName));
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

        private bool IsTurretAndAnotherTarget(GenericShip defender, IShipWeapon weapon, bool isSilent)
        {
            ShotInfo shotInfo = new ShotInfo(HostShip, defender, weapon);
            if (!shotInfo.ShotAvailableFromArcs.Any(a => a.ArcType == ArcType.SingleTurret))
            {
                if (!isSilent) Messages.ShowError("Your attack must use a turret arc");
                return false;
            }

            if (defender.ShipId == PreviousTarget.ShipId)
            {
                if (!isSilent) Messages.ShowError("Your attack must be against a ship you have not already attacked this round");
                return false;
            }

            return true;
        }
    }
}