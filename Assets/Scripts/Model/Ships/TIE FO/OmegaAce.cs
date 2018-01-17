//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
using Upgrade;
using Ship;
//using System;

namespace Ship
{
    namespace TIEFO
    {
        public class OmegaAce : TIEFO
        {
            public OmegaAce() : base ()
            {
                PilotName = "\"Omega Ace\"";
                PilotSkill = 7;
                Cost = 20;
                
                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                PilotAbilities.Add(new Abilities.OmegaAceAbility());
            }
        }
    }
}

namespace Abilities
{
    public class OmegaAceAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnCombatPhaseStart += RegisterOmegaAceAbility;
            HostShip.OnCombatPhaseEnd += RemoveOmegaAceAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnCombatPhaseStart -= RegisterOmegaAceAbility;
            HostShip.OnCombatPhaseEnd -= RemoveOmegaAceAbility;
        }

        private void RegisterOmegaAceAbility(GenericShip genericShip)
        {
            RegisterAbilityTrigger(TriggerTypes.OnImmediatelyAfterRolling, ShowDecision);
        }

        private void RemoveOmegaAceAbility(GenericShip genericShip)
        {
            // At the end of combat phase, need to remove attack value increase
            if (IsAbilityUsed)
            {
                //HostShip.ChangeFirepowerBy(-1);
                HostShip.AfterGotNumberOfPrimaryWeaponAttackDice -= ZetaLeaderAddAttackDice;
                IsAbilityUsed = false;
            }
        }


        private void ShowDecision(object sender, System.EventArgs e)
        {
            // check if this ship is stressed
            if (HostShip.HasToken(typeof(Tokens.FocusToken)) &&
                HostShip.HasToken(typeof(Tokens.BlueTargetLockToken)) &&
                Combat.Defender.HasToken(typeof(Tokens.RedTargetLockToken)))
            {
                // give user the option to use ability
                AskToUseAbility(ShouldUsePilotAbility, UseAbility);
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private bool ShouldUsePilotAbility()
        {
            return Actions.HasTarget(HostShip);
        }

        private void UseAbility(object sender, System.EventArgs e)
        {
            // don't need to check stressed as done already
            // add an attack dice
            IsAbilityUsed = true;
            //HostShip.ChangeFirepowerBy(+1);
            HostShip.AfterGotNumberOfPrimaryWeaponAttackDice += ZetaLeaderAddAttackDice;
            HostShip.AssignToken(new Tokens.StressToken(), SubPhases.DecisionSubPhase.ConfirmDecision);
        }

        
        private void ZetaLeaderAddAttackDice(ref int value)
        {
            value++;
        }
    }
}