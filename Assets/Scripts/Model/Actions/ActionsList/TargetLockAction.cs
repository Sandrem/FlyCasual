using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionsList
{

    public class TargetLockAction : GenericAction
    {
        public TargetLockAction() {
            Name = EffectName = "Target Lock";
        }

        public override void ActionEffect()
        {
            char letter = ' ';
            letter = Actions.GetTargetLocksLetterPair(Combat.Attacker, Combat.Defender);
            if (letter != ' ')
            {
                Game.Selection.ActiveShip.SpendToken(typeof(Tokens.BlueTargetLockToken), letter);
                Combat.Defender.RemoveToken(typeof(Tokens.RedTargetLockToken), letter);

                //TODO: 2 Kinds of reroll
                Dices.RerollDices(Combat.CurentDiceRoll, "failures");
            }
        }

        public override bool IsActionEffectAvailable()
        {
            bool result = false;
            if (Combat.AttackStep == CombatStep.Attack)
            {
                if (Actions.GetTargetLocksLetterPair(Combat.Attacker, Combat.Defender) != ' ')
                {
                    result = true;
                }
            }
            return result;
        }

        public override void ActionTake()
        {
            Phases.StartSelectTargetSubPhase("Select target for Target Lock");
        }

    }

}
