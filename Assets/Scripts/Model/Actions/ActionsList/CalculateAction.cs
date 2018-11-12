using System.Collections;
using System.Collections.Generic;
using Tokens;
using UnityEngine;

namespace ActionsList
{

    public class CalculateAction : GenericAction
    {

        public CalculateAction() {
            Name = DiceModificationName = "Calculate";

            TokensSpend.Add(typeof(Tokens.CalculateToken));
            IsTurnsOneFocusIntoSuccess = true;
            CanBeUsedFewTimes = true;
        }

        public override void ActionEffect(System.Action callBack)
        {
            Combat.CurrentDiceRoll.ApplyCalculate();
            Selection.ActiveShip.Tokens.SpendToken(typeof(Tokens.CalculateToken), callBack);
        }

        public override int GetDiceModificationPriority()
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
                        result = 41;
                    }
                }
            }

            if (Combat.AttackStep == CombatStep.Attack)
            {
                int attackFocuses = Combat.DiceRollAttack.Focuses;
                if (attackFocuses > 0)
                {
                    result = 41;
                }
            }

            return result;
        }

        public override void ActionTake()
        {
            Selection.ThisShip.Tokens.AssignToken(typeof(CalculateToken), Phases.CurrentSubPhase.CallBack);
        }

        public override int GetActionPriority()
        {
            int result = 0;

            // TODOREVERT if (Selection.ThisShip.UpgradeBar.HasUpgradeInstalled(typeof(UpgradesList.Expertise))) return 10;

            result = (ActionsHolder.HasTarget(Selection.ThisShip)) ? 50 : 20;
            return result;
        }

    }

}
