using Arcs;
using Ship;
using Abilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace TIESF
    {
        public class QuickDraw : TIESF
        {
            public QuickDraw() : base()
            {
                PilotName = "\"QuickDraw\"";
                PilotSkill = 9;
                Cost = 29;

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);


                PilotAbilities.Add(new Abilities.QuickDrawPilotAbility());
            }
        }
    }
}

// TODO: Test, Causes Crash After Second Attack. On shield lost needs a rework

namespace Abilities
{
    public class QuickDrawPilotAbility : GenericAbility
    {

        private bool performedRegularAttack;

        public override void ActivateAbility()
        {
            // Host Ship Registers Attack on Shield Lost 
            HostShip.OnShieldLost += CheckAbility;

            // Clear Ability On Shield Lost So Can Fire Again 
            Phases.OnCombatPhaseEnd += ClearIsAbilityUsedFlag;
        }


        public override void DeactivateAbility()
        {
            HostShip.OnShieldLost -= CheckAbility;
            Phases.OnRoundEnd -= ClearIsAbilityUsedFlag;
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
                Combat.Attacker.OnAttackFinishAsAttacker += StartCounterAttack;
            }
        }

        private void StartCounterAttack(GenericShip attacker)
        {
            Combat.Attacker.OnAttackFinishAsAttacker -= StartCounterAttack;

            RegisterAbilityTrigger(TriggerTypes.OnCombatCheckExtraAttack, RegisterCombat);
        }

        private void RegisterCombat(object sender, System.EventArgs e)
        {
            if (!IsAbilityUsed)
            {
                Messages.ShowInfo("\"Quick Draw\": Additional Combat is Engaged");

                // Save his "is already attacked" flag
                performedRegularAttack = HostShip.IsAttackPerformed;

                HostShip.OnAttackFinishAsAttacker += SetIsAbilityIsUsed;

                Combat.StartAdditionalAttack(
                    HostShip,
                    AfterExtraAttackSubPhase
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