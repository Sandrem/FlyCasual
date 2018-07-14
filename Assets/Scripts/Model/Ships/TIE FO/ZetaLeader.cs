using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ship;
using System;
using Tokens;

namespace Ship
{
    namespace TIEFO
    {
        public class ZetaLeader : TIEFO
        {
            public ZetaLeader () : base ()
            {
                PilotName = "\"Zeta Leader\"";
                PilotSkill = 7;
                Cost = 20;

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                PilotAbilities.Add(new Abilities.ZetaLeaderAbility());
            }
        }
    }
}

namespace Abilities
{
    public class ZetaLeaderAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnAttackStartAsAttacker += RegisterEpsilonLeaderAbility;
            HostShip.OnAttackFinishAsAttacker += RemoveEpsilonLeaderAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackStartAsAttacker -= RegisterEpsilonLeaderAbility;
            HostShip.OnAttackFinishAsAttacker -= RemoveEpsilonLeaderAbility;
        }

        private void RegisterEpsilonLeaderAbility()
        {
            RegisterAbilityTrigger(TriggerTypes.OnAttackStart, ShowDecision);
        }

        private void ShowDecision(object sender, System.EventArgs e)
        {
            // check if this ship is stressed
            if (!HostShip.Tokens.HasToken(typeof(StressToken)))
            {
                // give user the option to use ability
                AskToUseAbility(AlwaysUseByDefault, UseAbility);
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private void UseAbility(object sender, System.EventArgs e)
        {
            // don't need to check stressed as done already
            // add an attack dice
            IsAbilityUsed = true;
            //HostShip.ChangeFirepowerBy(+1);
            HostShip.AfterGotNumberOfPrimaryWeaponAttackDice += ZetaLeaderAddAttackDice;
            HostShip.Tokens.AssignToken(typeof(StressToken), SubPhases.DecisionSubPhase.ConfirmDecision);
        }

        private void RemoveEpsilonLeaderAbility(GenericShip genericShip)
        {
            // At the end of combat phase, need to remove attack value increase
            if (IsAbilityUsed)
            {
                //HostShip.ChangeFirepowerBy(-1);
                HostShip.AfterGotNumberOfPrimaryWeaponAttackDice -= ZetaLeaderAddAttackDice;
                IsAbilityUsed = false;
            }
        }
        private void ZetaLeaderAddAttackDice(ref int value)
        {
            value++;
        }
    }
}