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
        }

        public override void ActionEffect()
        {
            Dices.ApplyFocus(Combat.CurentDiceRoll);
            Selection.ActiveShip.SpendToken(typeof(Tokens.FocusToken));
        }

        public override void ActionTake()
        {
            Selection.ThisShip.AssignToken(new Tokens.FocusToken());
            Phases.CurrentSubPhase.callBack();
        }

    }

}
