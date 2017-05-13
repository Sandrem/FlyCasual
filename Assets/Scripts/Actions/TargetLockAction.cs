using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Actions
{

    public class TargetLockAction : GenericAction
    {
        public TargetLockAction() {
            Name = EffectName = "Target Lock";
        }

        public override void ActionEffect()
        {
            //TODO: 2 Kinds of reroll
            Game.Combat.CurentDiceRoll.Reroll("failures");

            Game.Selection.ActiveShip.SpendToken(typeof(Tokens.BlueTargetLockToken));
            Game.Combat.Defender.RemoveToken(typeof(Tokens.RedTargetLockToken));
        }

        public override bool IsActionEffectAvailable()
        {
            bool result = false;
            if (Game.Combat.AttackStep == CombatStep.Attack)
            {
                //TODO: Also if letter is same
                if (Game.Combat.Defender.HasToken(typeof(Tokens.RedTargetLockToken)))
                {
                    result = true;
                }
                
            }
            return result;
        }

        public override void ActionTake()
        {
            Game.PhaseManager.StartTemporaryPhase("Select target for Target Lock");
        }

    }

}
