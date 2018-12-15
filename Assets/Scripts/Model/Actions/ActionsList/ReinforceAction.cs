using SubPhases;
using System.Collections;
using System.Collections.Generic;
using Tokens;
using UnityEngine;

namespace ActionsList
{

    public class ReinforceAction : GenericAction
    {
        private Direction aiBetterSide = Direction.None;

        public ReinforceAction()
        {
            Name = DiceModificationName = "Reinforce";
            ImageUrl = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/reference-cards/ReinforceAction.png";
        }

        public override void ActionTake()
        {
            ReinforceSideSubphase decisionSubphase = (ReinforceSideSubphase)Phases.StartTemporarySubPhaseNew(
                Name,
                typeof(ReinforceSideSubphase),
                Phases.CurrentSubPhase.CallBack
            );

            decisionSubphase.InfoText = "Select a side";
            decisionSubphase.RequiredPlayer = HostShip.Owner.PlayerNo;

            decisionSubphase.AddDecision(
                "Fore side",
                delegate { HostShip.Tokens.AssignToken(typeof(ReinforceForeToken), DecisionSubPhase.ConfirmDecision); },
                isCentered: true
            );

            decisionSubphase.AddDecision(
                "Aft side",
                delegate { HostShip.Tokens.AssignToken(typeof(ReinforceAftToken), DecisionSubPhase.ConfirmDecision); },
                isCentered: true
            );

            decisionSubphase.DefaultDecisionName = (aiBetterSide == Direction.Top) ? "Fore side" : "Aft side";

            decisionSubphase.Start();
        }

        public override int GetActionPriority()
        {
            int result = 0;

            int resultFore = 25 + 30 * ActionsHolder.CountEnemiesTargeting(HostShip, 1);
            int resultAft = 25 + 30 * ActionsHolder.CountEnemiesTargeting(HostShip, -1);

            aiBetterSide = (resultFore >= resultAft) ? Direction.Top : Direction.Bottom;

            result = Mathf.Max(resultFore, resultAft);

            return result;
        }

        private class ReinforceSideSubphase : DecisionSubPhase { }

    }

}
