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
            if (IsAbilityUsed)
            {
                return;
            }

            if (HostShip.IsCannotAttackSecondTime) return;

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
            if (!IsAbilityUsed)
            {
                // Temporary fix
                if (HostShip.IsDestroyed)
                {
                    Triggers.FinishTrigger();
                    return;
                }

                // Save his "is already attacked" flag
                performedRegularAttack = HostShip.IsAttackPerformed;

                HostShip.OnAttackFinishAsAttacker += SetIsAbilityIsUsed;

                Combat.StartAdditionalAttack(
                    HostShip,
                    AfterExtraAttackSubPhase,
                    null,
                    HostShip.PilotName,
                    "You may perform a primary weapon attack.",
                    HostShip
                );
            }
            else
            {
                Messages.ShowErrorToHuman(string.Format("{0} cannot attack one more time", HostShip.PilotName));
                Triggers.FinishTrigger();
            }
        }

        private void AfterExtraAttackSubPhase()
        {
            // Restore previous value of "is already attacked" flag
            HostShip.IsAttackPerformed = performedRegularAttack;

            // Set IsAbilityUsed only after attack that was successfully started
            HostShip.OnAttackFinishAsAttacker -= SetIsAbilityIsUsed;

            Triggers.FinishTrigger();
        }

    }
}
