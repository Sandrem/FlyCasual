using Ship;
using System;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIEAgAggressor
    {
        public class DoubleEdge : TIEAgAggressor, TIE
        {
            public DoubleEdge() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "\"Double Edge\"",
                    2,
                    33,
                    limited: 1,
                    abilityType: typeof(Abilities.SecondEdition.DoubleEdgeAbility),
                    extraUpgradeIcon: UpgradeType.Elite,
                    seImageNumber: 128
                );
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class DoubleEdgeAbility : GenericAbility
    {
        private IShipWeapon AlreadyUsedWeapon;

        public override void ActivateAbility()
        {
            HostShip.OnAttackMissedAsAttacker += CheckDoubleEdgeAbility;
            Phases.Events.OnRoundEnd += ClearIsAbilityUsedFlag;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackMissedAsAttacker -= CheckDoubleEdgeAbility;
            Phases.Events.OnRoundEnd -= ClearIsAbilityUsedFlag;
        }

        private void CheckDoubleEdgeAbility()
        {
            if (!IsAbilityUsed && !HostShip.IsCannotAttackSecondTime && HasAlternativeWeapon() && WeaponIsTurretOrMissile())
            {
                IsAbilityUsed = true;
                AlreadyUsedWeapon = Combat.ChosenWeapon;

                // Trigger must be registered just before it's resolution
                HostShip.OnCombatCheckExtraAttack += RegisterDoubleEdgeAbility;
            }
        }

        private bool WeaponIsTurretOrMissile()
        {
            bool result = false;

            GenericUpgrade usedWeaponUpgrade = Combat.ChosenWeapon as GenericUpgrade;
            if (usedWeaponUpgrade == null) return false;

            if (usedWeaponUpgrade.UpgradeInfo.HasType(UpgradeType.Turret) || usedWeaponUpgrade.UpgradeInfo.HasType(UpgradeType.Missile))
            {
                result = true;
            }

            return result;
        }

        private bool HasAlternativeWeapon()
        {
            return HostShip.UpgradeBar.GetUpgradesOnlyFaceup().Any(n => (n as IShipWeapon) != null);
        }

        private void RegisterDoubleEdgeAbility(GenericShip ship)
        {
            HostShip.OnCombatCheckExtraAttack -= RegisterDoubleEdgeAbility;

            RegisterAbilityTrigger(TriggerTypes.OnCombatCheckExtraAttack, UseDoubleEdgeAbility);
        }

        private void UseDoubleEdgeAbility(object sender, System.EventArgs e)
        {
            if (!HostShip.IsCannotAttackSecondTime)
            {
                Combat.StartAdditionalAttack(
                    HostShip,
                    FinishAdditionalAttack,
                    IsAnotherWeapon,
                    "\"Double Edge\"",
                    "You may perform a bonus attack using different weapon.",
                    HostShip
                );
            }
            else
            {
                Messages.ShowErrorToHuman(string.Format("{0} cannot attack one more time", HostShip.PilotName));
                Triggers.FinishTrigger();
            }
        }

        private void FinishAdditionalAttack()
        {
            // If attack is skipped, set this flag, otherwise regular attack can be performed second time
            Selection.ThisShip.IsAttackPerformed = true;

            Triggers.FinishTrigger();
        }

        private bool IsAnotherWeapon(GenericShip defender, IShipWeapon weapon, bool isSilent)
        {
            bool result = false;

            if (weapon != AlreadyUsedWeapon)
            {
                result = true;
            }
            else
            {
                if (!isSilent) Messages.ShowError("Attack must be performed using another weapon");
            }

            return result;
        }
    }
}
