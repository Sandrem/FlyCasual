using Editions;
using System.Collections;
using System.Collections.Generic;
using Tokens;
using UnityEngine;

namespace ActionsList
{

    public class FocusAction: GenericAction
    {

        public FocusAction() {
            Name = DiceModificationName = "Focus";

            TokensSpend.Add(typeof(Tokens.FocusToken));
            IsTurnsAllFocusIntoSuccess = true;
        }

        public override void ActionEffect(System.Action callBack)
        {
            Combat.CurrentDiceRoll.ApplyFocus();
            Selection.ActiveShip.Tokens.SpendToken(typeof(Tokens.FocusToken), callBack);
        }

        public override bool IsDiceModificationAvailable()
        {
            return Edition.Current is Editions.FirstEdition || Combat.CurrentDiceRoll.Focuses != 0;
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
            Selection.ThisShip.Tokens.AssignToken(typeof(FocusToken), Phases.CurrentSubPhase.CallBack);
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
