using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionsList
{

    public class EvadeAction : GenericAction
    {

        public EvadeAction() {
            Name = EffectName = "Evade";
        }

        public override void ActionEffect()
        {
            Dices.ApplyEvade(Combat.CurentDiceRoll);
            Selection.ActiveShip.SpendToken(typeof(Tokens.EvadeToken));
        }

        public override bool IsActionEffectAvailable()
        {
            bool result = false;
            if (Combat.AttackStep == CombatStep.Defence) result = true;
            return result;
        }

        public override void ActionTake(System.Action callBack)
        {
            Selection.ThisShip.AssignToken(new Tokens.EvadeToken());
            callBack();
        }

    }

}
