using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Ship;
using SubPhases;
using Tokens;
using Upgrade;

namespace Ship
{
    namespace FirstEdition.XWing
    {
        public class WesJanson : XWing
        {
            public WesJanson() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Wes Janson",
                    8,
                    29,
                    isLimited: true,
                    abilityType: typeof(Abilities.FirstEdition.WesJansonAbility),
                    extraUpgradeIcon: UpgradeType.Elite
                );
            }
        }
    }
}

namespace Abilities.FirstEdition
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
            RegisterAbilityTrigger(TriggerTypes.OnAttackFinish, StartSubphaseForWesJansonPilotAbility);
        }

        private void StartSubphaseForWesJansonPilotAbility(object sender, System.EventArgs e)
        {
            //grab a list of tokens that wes can remove
            var wes_list = Combat.Defender.Tokens.GetAllTokens()
               .Where(t => t is BlueTargetLockToken || t is FocusToken || t is EvadeToken)
               .ToList();

            //don't bother with the ability if the defender has no tokens to remove.
            if (wes_list.Count > 0)
            {
                var pilotAbilityDecision = (WesJansonDecisionSubPhase)Phases.StartTemporarySubPhaseNew(
                    Name,
                    typeof(WesJansonDecisionSubPhase),
                    Triggers.FinishTrigger
                );

                pilotAbilityDecision.InfoText = "Use Wes Janson's ability?";

                pilotAbilityDecision.ShowSkipButton = true;

                //helper variables
                int numTL = 1;
                int numFT = 1;
                int numET = 1;
                var name = "default";

                //depending on type of token, change the text box
                wes_list.ForEach(i => {
                    if (i is BlueTargetLockToken)
                    {
                        name = "Remove Target Lock";
                        if (numTL >= 2)
                        {
                            name += " " + numTL;
                        }
                        numTL++;
                    }
                    else if (i is FocusToken)
                    {
                        name = "Remove Focus Token";
                        if (numFT >= 2)
                        {
                            name += " " + numFT;
                        }
                        numFT++;
                    }
                    else if (i is EvadeToken)
                    {
                        name = "Remove Evade Token";
                        if (numET >= 2)
                        {
                            name += " " + numET;
                        }
                        numET++;
                    }
                    //add appropriate choice for player
                    pilotAbilityDecision.AddDecision(name, delegate { UseWesJansonAbility(i); });
                });

                //AI presses first button in decision dialog
                pilotAbilityDecision.DefaultDecisionName = pilotAbilityDecision.GetDecisions().First().Name;

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
            Messages.ShowInfo(string.Format("{0} removed {1} from {2}", HostShip.PilotName, token.Name, Combat.Defender.PilotName));

            Combat.Defender.Tokens.RemoveToken(token, DecisionSubPhase.ConfirmDecision);
        }

        private void DontUseWesJansonAbility(object sender, System.EventArgs e)
        {
            DecisionSubPhase.ConfirmDecision();
        }

        private class WesJansonDecisionSubPhase : DecisionSubPhase { }
    }
}
