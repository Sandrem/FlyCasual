using Content;
using Ship;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIESfFighter
    {
        public class Quickdraw : TIESfFighter
        {
            public Quickdraw() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "\"Quickdraw\"",
                    "Defiant Duelist",
                    Faction.FirstOrder,
                    6,
                    6,
                    25,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.QuickDrawPilotAbility),
                    charges: 1,
                    regensCharges: 1,
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Talent,
                        UpgradeType.Talent,
                        UpgradeType.Modification
                    },
                    tags: new List<Tags>
                    {
                        Tags.Tie
                    }
                );

                ImageUrl = "https://squadbuilder.fantasyflightgames.com/card_images/en/d038dadd7a62bbe2de89d3866e1a3639.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class QuickDrawPilotAbility : GenericAbility
    {
        private bool performedRegularAttack;

        public override void ActivateAbility()
        {
            // Host Ship Registers Attack on Shield Lost 
            HostShip.OnShieldLost += CheckAbility;

            // Clear Ability On Shield Lost So Can Fire Again 
            Phases.Events.OnCombatPhaseEnd_NoTriggers += ClearIsAbilityUsedFlag;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnShieldLost -= CheckAbility;
            Phases.Events.OnCombatPhaseEnd_NoTriggers -= ClearIsAbilityUsedFlag;
        }

        private void CheckAbility()
        {
            if (IsAbilityCanBeUsed()) InitializeCounterAttack();
        }

        protected virtual bool IsAbilityCanBeUsed()
        {
            if (HostShip.State.Charges == 0 || HostShip.IsCannotAttackSecondTime) return false;

            return true;
        }

        private void InitializeCounterAttack()
        {
            if (Combat.AttackStep == CombatStep.None)
            {
                RegisterAbilityTrigger(TriggerTypes.OnShieldIsLost, RegisterCombat);
            }
            else
            {
                Combat.Attacker.OnCombatDeactivation += StartCounterAttack;
            }
        }

        private void StartCounterAttack(GenericShip ship)
        {
            ship.OnCombatDeactivation -= StartCounterAttack;

            RegisterAbilityTrigger(TriggerTypes.OnCombatCheckExtraAttack, RegisterCombat);
        }

        private void RegisterCombat(object sender, System.EventArgs e)
        {
            if (IsAbilityCanBeUsed())
            {
                // Save his "is already attacked" flag
                performedRegularAttack = HostShip.IsAttackPerformed;

                HostShip.OnAttackStartAsAttacker += MarkAbilityAsUsed;

                HostShip.IsCannotAttackSecondTime = true;

                Combat.StartSelectAttackTarget(
                    HostShip,
                    AfterExtraAttackSubPhase,
                    IsPrimaryWeaponAttack,
                    HostShip.PilotInfo.PilotName,
                    "You may perform a primary weapon attack",
                    HostShip
                );
            }
            else
            {
                Messages.ShowErrorToHuman(string.Format("{0} cannot attack an additional time", HostShip.PilotInfo.PilotName));
                Triggers.FinishTrigger();
            }
        }

        private bool IsPrimaryWeaponAttack(GenericShip targetShip, IShipWeapon weapon, bool isSilent)
        {
            bool result = false;

            if (weapon.WeaponType == WeaponTypes.PrimaryWeapon)
            {
                result = true;
            }
            else
            {
                if (!isSilent) Messages.ShowError("This attack must be performed using your primary weapon");
            }

            return result;
        }

        private void AfterExtraAttackSubPhase()
        {
            // Restore previous value of "is already attacked" flag
            HostShip.IsAttackPerformed = performedRegularAttack;

            // Set IsAbilityUsed only after attack that was successfully started
            HostShip.OnAttackStartAsAttacker -= MarkAbilityAsUsed;

            //if bonus attack was skipped, allow bonus attacks again
            if (HostShip.IsAttackSkipped) HostShip.IsCannotAttackSecondTime = false;

            Triggers.FinishTrigger();
        }

        protected virtual void MarkAbilityAsUsed()
        {
            HostShip.SpendCharge();
        }

    }
}
