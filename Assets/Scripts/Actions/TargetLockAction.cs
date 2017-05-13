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
            char letter = ' ';
            letter = Game.Actions.GetTargetLocksLetterPair(Game.Combat.Attacker, Game.Combat.Defender);
            if (letter != ' ')
            {
                Game.Selection.ActiveShip.SpendToken(typeof(Tokens.BlueTargetLockToken), letter);
                Game.Combat.Defender.RemoveToken(typeof(Tokens.RedTargetLockToken), letter);

                //TODO: 2 Kinds of reroll
                Game.Dices.RerollDices(Game.Combat.CurentDiceRoll, "failures");
            }
        }

        public override bool IsActionEffectAvailable()
        {
            bool result = false;
            if (Game.Combat.AttackStep == CombatStep.Attack)
            {
                if (Game.Actions.GetTargetLocksLetterPair(Game.Combat.Attacker, Game.Combat.Defender) != ' ')
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
