using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Ship;
using SubPhases;
using System;
using Tokens;

namespace Ship
{
    namespace XWing
    {
        public class WesJanson : XWing
        {
            public WesJanson() : base()
            {
                PilotName = "Wes Janson";
                PilotSkill = 8;
                Cost = 29;

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                PilotAbilities.Add(new Abilities.WesJansonAbility());
            }
        }
    }
}

namespace Abilities
{
    public class WesJansonAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnAttackFinishAsAttacker += WesJansonPilotAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackFinishAsAttacker -= WesJansonPilotAbility;
        }

        public void WesJansonPilotAbility(GenericShip ship)
        {
			RegisterAbilityTrigger(TriggerTypes.OnAttackFinishAsAttacker, StartSubphaseForWesJansonPilotAbility);
        }

		private void StartSubphaseForWesJansonPilotAbility(object sender, System.EventArgs e)
        {
			//don't bother with the ability if the defender has no tokens.
            if (Combat.Defender.HasToken(typeof(Tokens.BlueTargetLockToken), '*') || Combat.Defender.HasToken(typeof(Tokens.FocusToken)) || Combat.Defender.HasToken(typeof(Tokens.EvadeToken)))
            {
                var pilotAbilityDecision = (WesJansonDecisionSubPhase)Phases.StartTemporarySubPhaseNew(
                    Name,
                    typeof(WesJansonDecisionSubPhase),
                    Triggers.FinishTrigger
                );

                pilotAbilityDecision.InfoText = "Use Wes Janson's ability?";

				//grab a list of tokens that wes can remove
                var wes_list = Combat.Defender.GetAssignedTokens()
                   .Where(t => t is Tokens.BlueTargetLockToken || t is Tokens.FocusToken || t is Tokens.EvadeToken)
                   .ToList();

                pilotAbilityDecision.AddDecision("No", DontUseWesJansonAbility);

				//helper variables
				int numTL = 1;
				int numFT = 1;
				int numET = 1;
				var name = "default";
				
				//depending on type of token, change the text box
                wes_list.ForEach(i => {
					if(i is Tokens.BlueTargetLockToken){
						name = "Remove Target Lock";
						if(numTL >= 2){
							name += " " + numTL;
						}
						numTL++;
					}else if (i is Tokens.FocusToken){
						name = "Remove Focus Token";
						if(numFT >= 2){
							name += " " + numFT;
						}
						numFT++;
					}else if (i is Tokens.EvadeToken){
						name = "Remove Evade Token" + numET;
						if(numET >= 2){
							name += " " + numET;
						}
						numET++;
					}
                    //add appropriate choice for player
                    pilotAbilityDecision.AddDecision(name, delegate { UseWesJansonAbility(i); });
                });

                pilotAbilityDecision.DefaultDecision = "No";

                pilotAbilityDecision.Start();
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }
		
		private void UseWesJansonAbility(GenericToken token)
        {
            //remove the chosen token
			Combat.Defender.RemoveToken(token);
			DecisionSubPhase.ConfirmDecision();
        }
		
		private void DontUseWesJansonAbility(object sender, System.EventArgs e)
        {
            DecisionSubPhase.ConfirmDecision();
        }
		
		private class WesJansonDecisionSubPhase : DecisionSubPhase
        {
            public override void SkipButton()
            {
                ConfirmDecision();
            }
        }
    }
}