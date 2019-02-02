using Ship;
using Upgrade;

namespace Ship
{
    namespace FirstEdition.TIESfFighter
    {
        public class QuickDraw : TIESfFighter
        {
            public QuickDraw() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "\"QuickDraw\"",
                    9,
                    29,
                    isLimited: true,
                    abilityType: typeof(Abilities.FirstEdition.QuickDrawPilotAbility),
                    extraUpgradeIcon: UpgradeType.Talent
                );
            }
        }
    }
}

namespace Abilities.FirstEdition
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
            Phases.Events.OnRoundEnd -= ClearIsAbilityUsedFlag;
        }

        private void CheckAbility()
        {
            if (IsAbilityCanBeUsed()) InitializeCounterAttack();
        }

        protected virtual bool IsAbilityCanBeUsed()
        {
            if (IsAbilityUsed || HostShip.IsCannotAttackSecondTime) return false;

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
                Combat.Attacker.OnCombatCheckExtraAttack += StartCounterAttack;
            }
        }

        private void StartCounterAttack(GenericShip ship)
        {
            ship.OnCombatCheckExtraAttack -= StartCounterAttack;

            RegisterAbilityTrigger(TriggerTypes.OnCombatCheckExtraAttack, RegisterCombat);
        }

        private void RegisterCombat(object sender, System.EventArgs e)
        {
            if (IsAbilityCanBeUsed())
            {
                // Temporary fix
                if (HostShip.IsDestroyed)
                {
                    Triggers.FinishTrigger();
                    return;
                }

                // Save his "is already attacked" flag
                performedRegularAttack = HostShip.IsAttackPerformed;

                HostShip.OnAttackStartAsAttacker += MarkAbilityAsUsed;

                Combat.StartAdditionalAttack(
                    HostShip,
                    AfterExtraAttackSubPhase,
                    IsPrimaryWeaponAttack,
                    HostShip.PilotInfo.PilotName,
                    "You may perform a primary weapon attack.",
                    HostShip
                );
            }
            else
            {
                Messages.ShowErrorToHuman(string.Format("{0} cannot attack one more time", HostShip.PilotInfo.PilotName));
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
                if (!isSilent) Messages.ShowError("Attack must be performed using primary weapon");
            }

            return result;
        }

        private void AfterExtraAttackSubPhase()
        {
            // Restore previous value of "is already attacked" flag
            HostShip.IsAttackPerformed = performedRegularAttack;

            // Set IsAbilityUsed only after attack that was successfully started
            HostShip.OnAttackStartAsAttacker -= MarkAbilityAsUsed;

            Triggers.FinishTrigger();
        }

        protected virtual void MarkAbilityAsUsed()
        {
            SetIsAbilityIsUsed(HostShip);
        }

    }
}
