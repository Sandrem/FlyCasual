using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionsList
{

    public class EvadeAction : GenericAction
    {

        public EvadeAction() {
            Name = EffectName = "Evade";

            TokensSpend.Add(typeof(Tokens.EvadeToken));
        }

        public override void ActionEffect(System.Action callBack)
        {
            Combat.CurrentDiceRoll.ApplyEvade();
            Selection.ActiveShip.Tokens.SpendToken(typeof(Tokens.EvadeToken), callBack);
        }

        public override bool IsActionEffectAvailable()
        {
            bool result = false;
            if (Combat.AttackStep == CombatStep.Defence) result = true;
            return result;
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
                    result = (attackSuccessesCancelable - defenceSuccesses == 1) ? 70 : 20;
                }
            }

            return result;
        }

        public override void ActionTake()
        {
            Selection.ThisShip.Tokens.AssignToken(new Tokens.EvadeToken(Selection.ThisShip), Phases.CurrentSubPhase.CallBack);
        }

        public override int GetActionPriority()
        {
            int result = 0;
            result = 40;
            return result;
        }

    }

}
