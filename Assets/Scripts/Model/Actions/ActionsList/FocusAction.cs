using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionsList
{

    public class FocusAction: GenericAction
    {

        public FocusAction() {
            Name = EffectName = "Focus";

            TokensSpend.Add(typeof(Tokens.FocusToken));
            IsTurnsAllFocusIntoSuccess = true;
        }

        public override void ActionEffect(System.Action callBack)
        {
            Combat.CurrentDiceRoll.ApplyFocus();
            Selection.ActiveShip.Tokens.SpendToken(typeof(Tokens.FocusToken), callBack);
        }

        public override int GetActionEffectPriority()
        {
            int result = 0;

            if (Combat.AttackStep == CombatStep.Defence)
            {
                int attackSuccessesCancelable = Combat.DiceRollAttack.SuccessesCancelable;
                int defenceSuccesses = Combat.DiceRollDefence.Successes;
                if (attackSuccessesCancelable > defenceSuccesses)
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
            Selection.ThisShip.Tokens.AssignToken(new Tokens.FocusToken(Selection.ThisShip), Phases.CurrentSubPhase.CallBack);
        }

        public override int GetActionPriority()
        {
            int result = 0;

            if (Selection.ThisShip.UpgradeBar.HasUpgradeInstalled(typeof(UpgradesList.Expertise))) return 10;

            result = (Actions.HasTarget(Selection.ThisShip)) ? 50 : 20;
            return result;
        }

    }

}
