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
                int defenceSuccesses = Combat.CurrentDiceRoll.Successes;
                if (attackSuccessesCancelable > defenceSuccesses)
                {
                    int defenceFocuses = Combat.CurrentDiceRoll.Focuses;
                    int numFocusTokens = Selection.ActiveShip.Tokens.CountTokensByType(typeof(FocusToken));
                    if (numFocusTokens > 0 && defenceFocuses > 1)
                    {
                        // Multiple focus results on our defense roll and we have a Focus token.  Use it instead of the Calculate.
                        result = 0;
                    }
                    else if (defenceFocuses > 0)
                    {
                        // We don't have a focus token.  Better use the Calculate.
                        result = 41;
                    }
                }
            }

            if (Combat.AttackStep == CombatStep.Attack)
            {
                int attackFocuses = Combat.CurrentDiceRoll.Focuses;
                if (attackFocuses > 0)
                {
                    result = 41;
                }
            }

            return result;
        }

        public override bool IsDiceModificationAvailable()
        {
            return Combat.CurrentDiceRoll.Focuses != 0;
        }

        public override void ActionTake()
        {
            Selection.ThisShip.Tokens.AssignToken(typeof(CalculateToken), Phases.CurrentSubPhase.CallBack);
        }

        public override int GetActionPriority()
        {
            int result = 0;

            if (Selection.ThisShip.UpgradeBar.HasUpgradeInstalled(typeof(UpgradesList.FirstEdition.Expertise))) return 10;

            result = (ActionsHolder.HasTarget(Selection.ThisShip)) ? 50 : 20;
            return result;
        }

    }

}
