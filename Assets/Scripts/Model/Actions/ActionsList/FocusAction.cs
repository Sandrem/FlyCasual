using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionsList
{

    public class FocusAction: GenericAction
    {

        public FocusAction() {
            Name = EffectName = "Focus";

            IsSpendFocus = true;
            IsTurnsAllFocusIntoSuccess = true;
        }

        public override void ActionEffect(System.Action callBack)
        {
            Combat.CurentDiceRoll.ApplyFocus();
            Selection.ActiveShip.SpendToken(typeof(Tokens.FocusToken));
            callBack();
        }

        public override int GetActionEffectPriority()
        {
            int result = 0;

            if (Combat.AttackStep == CombatStep.Defence)
            {
                int attackSuccesses = Combat.DiceRollAttack.Successes;
                int defenceSuccesses = Combat.DiceRollDefence.Successes;
                if (attackSuccesses > defenceSuccesses)
                {
                    int defenceFocuses = Combat.DiceRollDefence.Focuses;
                    if (defenceFocuses > 0)
                    {
                        result = (defenceFocuses > 1) ? 50 : 40;
                    }
                }
            }

            if (Combat.AttackStep == CombatStep.Attack)
            {
                int attackFocuses = Combat.DiceRollAttack.Focuses;
                if (attackFocuses > 0)
                {
                    result = (attackFocuses > 1) ? 50 : 40;
                }
            }

            return result;
        }

        public override void ActionTake()
        {
            Selection.ThisShip.AssignToken(new Tokens.FocusToken(), Phases.CurrentSubPhase.CallBack);
        }

        public override int GetActionPriority()
        {
            int result = 0;
            result = (Actions.HasTarget(Selection.ThisShip)) ? 50 : 20;
            return result;
        }

    }

}
