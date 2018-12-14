using System.Collections;
using System.Collections.Generic;
using Tokens;
using UnityEngine;

namespace ActionsList
{

    public class ReinforceAction : GenericAction
    {

        public ReinforceAction()
        {
            Name = DiceModificationName = "Reinforce";
            ImageUrl = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/reference-cards/ReinforceAction.png";
        }

        public override void ActionTake()
        {
            // TODO: Ask for direction

            HostShip.Tokens.AssignToken(typeof(ReinforceForeToken), Phases.CurrentSubPhase.CallBack);
        }

        public override int GetActionPriority()
        {
            int result = 0;

            int resultFore = 25 + 30 * ActionsHolder.CountEnemiesTargeting(HostShip, 1);
            int resultAft = 25 + 30 * ActionsHolder.CountEnemiesTargeting(HostShip, -1);

            result = Mathf.Max(resultFore, resultAft);

            return result;
        }

    }

}
