﻿using BoardTools;
using Ship;
using System.Linq;
using Tokens;
using UnityEngine;

namespace ActionsList
{

    public class ReinforceForeAction : GenericReinforceAction
    {

        public ReinforceForeAction()
        {
            Name = DiceModificationName = "Reinforce (Fore)";
            Facing = Arcs.ArcFacing.Front180;
        }

        public override void ActionTake()
        {
            base.ActionTake();
            Host.Tokens.AssignToken(typeof(ReinforceForeToken), Phases.CurrentSubPhase.CallBack);
        }

        public override int GetActionPriority()
        {
            int result = 0;

            result = 25 + 30*Actions.CountEnemiesTargeting(Host, 1);

            return result;
        }

    }

}
