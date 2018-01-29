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

            // Clear Ability On Shield Lost So Can Fire Again 
            Phases.OnCombatPhaseEnd += ClearIsAbilityUsedFlag;
            // Host Ship Registers Attack on Shield Lost 
            HostShip.OnShieldLost += CheckAbility;

        }


        public override void DeactivateAbility()
        {

            HostShip.OnShieldLost -= CheckAbility;
            Phases.OnRoundEnd -= ClearIsAbilityUsedFlag;

        }


        private void CheckAbility()
        {
            if (IsAbilityUsed || HostShip.Shields == 0)
            {
                return;
            }

            if (HostShip.IsCannotAttackSecondTime) return;

            RegisterAbilityTrigger(TriggerTypes.OnShieldIsLost, RegisterCombat);
        }

        private void RegisterCombat(object sender, System.EventArgs e)
        {
            if (HostShip.Shields > 0)
            {

                Messages.ShowInfo("\"Quick Draw\": Additional Combat is Engaged");

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

            HostShip.OnAttackFinishAsAttacker += SetIsAbilityIsUsed;

            Phases.CallCombatPhaseEndTrigger();
            Triggers.FinishTrigger();

        }

    }
}